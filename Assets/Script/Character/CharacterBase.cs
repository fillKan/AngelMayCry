using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
	public enum eState
	{
		Idle,
		Move,
		Attack,
		Hit,
		Down,
		Wake,
		Dead,
		End
	}

	public enum eCounterAttackState
	{
		None,
		Player_Sword_Parrying
	}

	[SerializeField] protected float _MaxHp = 100;
	protected float _Hp;

	[Tooltip("슈퍼아머는 단순히 bool이 아니라 플로트로 하고 0보다 큰지로 판단")]
	[SerializeField] protected float _MaxSuperArmor = 0;
	protected float _SuperArmor;

	[Tooltip("0.5보다 작으면 y축 넉백 무시")]
	[SerializeField] protected float _KnockBackMultiplier = 1;
	[SerializeField] protected float _DamageMultiplier = 1;

	protected Rigidbody2D _Rigidbody;
	protected Animator _Animator;
	protected eState _State = eState.Idle;
	protected eCounterAttackState _CounterAttackState = eCounterAttackState.None;
	protected bool _isInAir = true;

	protected delegate void OnStateEnter();
	protected OnStateEnter _OnIdle = null;
	protected OnStateEnter _OnHit = null;
	protected OnStateEnter _OnDeath = null;
	protected System.Action<float, float, Vector2, GameObject> _OnAttackCountered;

	private IEnumerator _StunTimeRoutine = null;
	protected float _CurYVel = 0;

	protected virtual void Awake()
	{
		_Rigidbody = GetComponent<Rigidbody2D>();
		_Animator = GetComponent<Animator>();
	}

	protected virtual void OnEnable()
	{
		_Hp = _MaxHp;
		_SuperArmor = _MaxSuperArmor;
		_KnockBackMultiplier = 1;
		_DamageMultiplier = 1;
		_State = eState.Idle;
		_isInAir = true;
	}

	protected virtual void Update()
	{
		if(_Rigidbody.velocity.y <= -8 && _State == eState.Hit)
		{
			_Animator.Play("Fall");
		}
	}

	public void FixedUpdate()
	{
		if(_Rigidbody.velocity.y != 0)
			_CurYVel = _Rigidbody.velocity.y;
	}

	protected virtual void OnCollisionStay2D(Collision2D collision)
	{
		switch(collision.gameObject.tag)
		{
			case "Ground":
				{
					if (_isInAir == false || (_isInAir == true && _CurYVel > 0))
						break;

					var contacts = collision.contacts;
					foreach (var contactPoint in contacts)
					{
						if (contactPoint.normal.y > 0)
						{
							if (_CurYVel > -10)
							{
								_isInAir = false;
								if (_State == eState.Hit || _State == eState.Dead)
									SetState(eState.Down);
								else if (_State == eState.Idle)
									SetState(eState.Idle);
							}
							else
							{
								_Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, _CurYVel * -25f * Time.unscaledDeltaTime);
								transform.Translate(0, 0.01f, 0);
								_CurYVel = 0;
							}

							break;
						}
					}
					break;
				}
		}
	}

	public virtual void DealDamage(float damage, float stunTime, Vector2 knockBack, GameObject from)
	{
		if (_State == eState.Down || _State == eState.Dead || _State == eState.Wake)
			return;
		if (_CounterAttackState != eCounterAttackState.None)
		{
			_OnAttackCountered(damage, stunTime, knockBack, from);
			return;
		}
		_Hp -= damage * _DamageMultiplier;
		AddForce(knockBack);
		GetComponent<SpriteRenderer>().material.SetInt("_isBlinking", 1);
		StartCoroutine(HitBlinkingRoutine());
		if(_Hp <= 0)
		{
			Death();
			return;
		}
		_SuperArmor -= damage;
		if(_SuperArmor <= 0 && _MaxSuperArmor != 0)
		{
			OnSuperArmorBreak();
			return;
		}

		if (_SuperArmor > 0 || stunTime == 0)
			return;

		SetState(eState.Hit);
		Vector2 scale = transform.localScale;
		scale.x = Mathf.Abs(scale.x) * -Mathf.Sign(from.transform.localScale.x);
		transform.localScale = scale;
		if (_StunTimeRoutine != null)
			StopCoroutine(_StunTimeRoutine);
		StartCoroutine(_StunTimeRoutine = StunTimeRoutine(knockBack.y == 0 ? stunTime : 0.01f));
	}

	public virtual void Death()
	{
		_OnDeath?.Invoke();
		if (!_isInAir)
		{
			AddForce(new Vector2(200 * (Mathf.Sign(transform.localScale.x) == 1 ? -1 : 1), 200));
		}
		SetState(eState.Hit);
		_State = eState.Dead;
	}

	private IEnumerator CorpseDisappearRoutine()
	{
		yield return new WaitForSeconds(2f);

		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		Color color = Color.white;
		while (renderer.color.a > 0)
		{
			color.a -= 0.3f * Time.deltaTime;
			renderer.color = color;
			yield return null;
		}

		gameObject.SetActive(false);
	}

	public virtual void OnSuperArmorBreak()
	{
		SetState(eState.Hit);
		StartCoroutine(HitBlinkingRoutine(0.15f));
		StartCoroutine(StunTimeRoutine(3));
		TimeManager.Instance.HitStop(0.5f);
	}

	private IEnumerator StunTimeRoutine(float time)
	{
		yield return new WaitForSeconds(time);
		yield return new WaitUntil(() => { return GetIsInAir() == false; });
		if(_SuperArmor <= 0)
			_SuperArmor = _MaxSuperArmor;
		if (_State == eState.Hit && !_isInAir)
			SetState(eState.Idle);
		_StunTimeRoutine = null;
	}

	private IEnumerator WakeRoutine()
	{
		yield return new WaitForSeconds(Random.Range(1f, 1.5f));
		SetState(eState.Wake);
	}

	private IEnumerator HitBlinkingRoutine(float time = 0.01f)
	{
		yield return new WaitForSecondsRealtime(time);

		GetComponent<SpriteRenderer>().material.SetInt("_isBlinking", 0);
	}

	public bool GetIsInAir()
	{
		return _isInAir;
	}

	public void AddForce(Vector2 force)
	{
		force.x *= _KnockBackMultiplier;
		if (_KnockBackMultiplier <= 0.5)
			force.y = 0;
		if (force.y != 0)
		{
			_CurYVel = force.y * Time.unscaledDeltaTime;
			_isInAir = true;
		}
		_Rigidbody.AddForce(force);
	}

	public void SetState(eState state)
	{
		_CounterAttackState = eCounterAttackState.None;
		switch(state)
		{
			case eState.Idle:
				_Animator.Play("Idle");
				_Animator.speed = 1;
				_OnIdle?.Invoke();
				break;

			case eState.Hit:
				_Animator.Play("Hit");
				_OnHit?.Invoke();
				break;

			case eState.Down:
				_Animator.Play("Down");
				if (_State == eState.Dead)
				{
					StartCoroutine(CorpseDisappearRoutine());
					state = eState.Dead;
				}
				else
					StartCoroutine(WakeRoutine());
				break;

			case eState.Wake:
				_Animator.Play("Wake");
				_Animator.speed = 1;
				break;
		}
		_State = state;
	}

	public void SetCounterAttackState(eCounterAttackState state)
	{
		_CounterAttackState = state;
	}

	public eState GetState()
	{
		return _State;
	}
}

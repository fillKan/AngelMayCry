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

	[SerializeField] protected float _MaxHp = 100;
	protected float _Hp;

	[Tooltip("���۾ƸӴ� �ܼ��� bool�� �ƴ϶� �÷�Ʈ�� �ϰ� 0���� ū���� �Ǵ�")]
	[SerializeField] protected float _MaxSuperArmor = 0;
	protected float _SuperArmor;

	[Tooltip("0.5���� ������ y�� �˹� ����")]
	[SerializeField] protected float _KnockBackMultiplier = 1;
	[SerializeField] protected float _DamageMultiplier = 1;

	protected Rigidbody2D _Rigidbody;
	protected Animator _Animator;
	protected eState _State = eState.Idle;
	protected bool _isInAir = true;

	protected delegate void OnStateEnter();
	protected OnStateEnter _OnIdle;
	protected OnStateEnter _OnHit;

	protected virtual void OnEnable()
	{
		_Hp = _MaxHp;
		_SuperArmor = _MaxSuperArmor;
		_Rigidbody = GetComponent<Rigidbody2D>();
		_Animator = GetComponent<Animator>();
		_KnockBackMultiplier = 1;
		_DamageMultiplier = 1;
		_State = eState.Idle;
		_isInAir = true;
	}

	protected virtual void Update()
	{
		if(_Rigidbody.velocity.y <= -10 && _State == eState.Hit)
		{
			_Animator.Play("Fall");
		}
	}

	protected virtual void OnCollisionStay2D(Collision2D collision)
	{
		switch(collision.gameObject.tag)
		{
			case "Ground":
				{
					if (_isInAir == false || (_isInAir == true && _Rigidbody.velocity.y > 0))
						break;

					var contacts = collision.contacts;
					foreach (var contactPoint in contacts)
					{
						if (contactPoint.normal.y > 0)
						{
							if (_Rigidbody.velocity.y > -10)
							{
								_isInAir = false;
								if (_State == eState.Hit)
									SetState(eState.Down);
								else
									SetState(eState.Idle);
							}
							else
							{
								Debug.Log("Bounce");
								_Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, _Rigidbody.velocity.y * -0.6f);
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
		_Hp -= damage * _DamageMultiplier;
		if(_Hp <= 0)
		{
			Death();
		}
		_SuperArmor -= damage;
		if(_SuperArmor <= 0 && _MaxSuperArmor != 0)
		{
			OnSuperArmorBreak();
			return;
		}

		knockBack.x *= _KnockBackMultiplier;
		if (_KnockBackMultiplier <= 0.5)
			knockBack.y = 0;
		if (knockBack.y != 0)
		{
			_isInAir = true;
			_Rigidbody.velocity = new Vector2(0, 0.1f);
		}
		_Rigidbody.AddForce(knockBack);
		GetComponent<SpriteRenderer>().material.SetInt("_isBlinking", 1);

		StartCoroutine(HitBlinkingRoutine());

		if (_SuperArmor > 0)
			return;

		SetState(eState.Hit);
		Vector2 scale = transform.localScale;
		scale.x = Mathf.Abs(scale.x) * -Mathf.Sign(from.transform.localScale.x);
		transform.localScale = scale;
		StartCoroutine(StunTimeRoutine(knockBack.y == 0 ? stunTime : 0.01f));
	}

	public virtual void Death()
	{

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
		_SuperArmor = _MaxSuperArmor;
		if (_State == eState.Hit)
			SetState(eState.Idle);
		else if (_State == eState.Down)
			SetState(eState.Wake);
	}

	private IEnumerator HitBlinkingRoutine(float time = 0.075f)
	{
		yield return new WaitForSecondsRealtime(time);

		GetComponent<SpriteRenderer>().material.SetInt("_isBlinking", 0);
	}

	public bool GetIsInAir()
	{
		return _isInAir;
	}

	public void SetState(eState state)
	{
		_State = state;
		switch(_State)
		{
			case eState.Idle:
				_Animator.Play("Idle");
				_Animator.speed = 1;
				_OnIdle?.Invoke();
				break;

			case eState.Hit:
				_Animator.Play("Hit");
				_Animator.speed = 0;
				_OnHit?.Invoke();
				break;

			case eState.Down:
				_Animator.Play("Down");
				_Animator.speed = 0.1f;
				break;

			case eState.Wake:
				_Animator.Play("Wake");
				_Animator.speed = 1;
				break;
		}
	}

	public eState GetState()
	{
		return _State;
	}
}

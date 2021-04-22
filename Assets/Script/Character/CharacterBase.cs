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

	[Tooltip("슈퍼아머는 단순히 bool이 아니라 플로트로 하고 0보다 큰지로 판단")]
	[SerializeField] protected float _MaxSuperArmor = 0;
	protected float _SuperArmor;

	[SerializeField] protected float _KnockBackMultiplier = 1;
	[SerializeField] protected float _DamageMultiplier = 1;

	protected Rigidbody2D _Rigidbody;
	protected Animator _Animator;
	protected eState _State = eState.Idle;

	public virtual void OnEnable()
	{
		_Hp = _MaxHp;
		_SuperArmor = _MaxSuperArmor;
		_Rigidbody = GetComponent<Rigidbody2D>();
		_Animator = GetComponent<Animator>();
		_KnockBackMultiplier = 1;
		_DamageMultiplier = 1;
		_State = eState.Idle;
	}

	public virtual void DealDamage(float damage, float stunTime, Vector2 knockBack, GameObject from)
	{
		_Hp -= damage * _DamageMultiplier;
		if(_Hp <= 0)
		{
			Death();
		}
		_SuperArmor -= damage;
		if(_SuperArmor <= 0)
		{
			OnSuperArmorBreak();
			return;
		}
		knockBack.x *= _KnockBackMultiplier;
		_Rigidbody.AddForce(knockBack);
		GetComponent<SpriteRenderer>().material.SetInt("_isBlinking", 1);

		StartCoroutine(HitBlinkingRoutine());

		if (_SuperArmor > 0)
			return;

		SetState(eState.Hit);
		StartCoroutine(StunTimeRoutine(stunTime));
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

		_SuperArmor = _MaxSuperArmor;
		SetState(eState.Idle);
	}

	private IEnumerator HitBlinkingRoutine(float time = 0.075f)
	{
		yield return new WaitForSecondsRealtime(time);

		GetComponent<SpriteRenderer>().material.SetInt("_isBlinking", 0);
	}

	public bool GetIsInAir()
	{
		return _Rigidbody.velocity.y != 0;
	}

	public void SetState(eState state)
	{
		_State = state;
		switch(_State)
		{
			case eState.Idle:
				_Animator.Play("Idle");
				_Animator.speed = 1;
				break;

			case eState.Hit:
				_Animator.Play("Hit");
				_Animator.speed = 0;
				break;
		}
	}

	public eState GetState()
	{
		return _State;
	}
}

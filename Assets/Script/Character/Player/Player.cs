using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterBase
{
    // Animation Transition //
    private const int Idle    = 0;
    private const int Move    = 1;
    private const int Jump    = 2;
    private const int Landing = 3;

    public Rigidbody2D Rigidbody => _Rigidbody;

    [Header("Move Property")]
    [SerializeField] private float _JumpForce;
    private bool _CanJump;

    [SerializeField] private float _MoveSpeed;
    [SerializeField] private float _MoveSpeedMax;
    private IEnumerator _MoveRoutine = null;

    [Header("Slip Property")] // 이동이 끝난 후 미끄러지는거
    [SerializeField, Range(0f, 3f)] private float _SlipTime;
    [SerializeField] private AnimationCurve _SlipCurve;

    [Header("Other Property")]
	private int _AnimatorHash;
	public string NextAnimation { get; set; }

    // 무기
    private WeaponBase _CurWeapon;
    private WeaponBase.eWeapons[] _EqiupedWeapons = new WeaponBase.eWeapons[5];
    private WeaponBase[] _WeaponDatas = new WeaponBase[(int)WeaponBase.eWeapons.End];

	protected override void Awake()
    {
		base.Awake();
        _CanJump = true;
        _AnimatorHash = _Animator.GetParameter(0).nameHash;
		_State = CharacterBase.eState.Idle;
        InitWeapons();

		_OnIdle = () =>
		{
			
		};

		_OnHit = () =>
		{
			if (_MoveRoutine != null)
			{
				StopCoroutine(_MoveRoutine);
				_MoveRoutine = null;
			}
		};

		_OnAttackCountered = (float damage, float stunTime, Vector2 knockback, GameObject from) =>
		{
			switch(_CounterAttackState)
			{
				case eCounterAttackState.Player_Sword_Parrying:
					NextAnimation = "Player_Sword_Parrying_Counter";
					_CounterAttackState = eCounterAttackState.None;
					break;
			}
		};
	}
	protected override void Update()
    {
		base.Update();
		if(NextAnimation != "")
		{
			_Animator.Play(NextAnimation);
			NextAnimation = "";
		}
        if (_State == CharacterBase.eState.Idle || _State == CharacterBase.eState.Move)
        {
			if (Input.GetKey(KeyCode.A))
			{
				if (_MoveRoutine == null)
				{
					_State = CharacterBase.eState.Move;
					MoveOrder(Vector2.left, () => Input.GetKeyUp(KeyCode.A));
				}
			}
			else if (Input.GetKey(KeyCode.D))
			{
				if (_MoveRoutine == null)
				{
					_State = CharacterBase.eState.Move;
					MoveOrder(Vector2.right, () => Input.GetKeyUp(KeyCode.D));
				}
			}

			if (Input.GetKeyDown(KeyCode.Space) && _CanJump)
            {
                _Rigidbody.AddForce(Vector2.up * _JumpForce, ForceMode2D.Impulse);
                _CanJump = false;
            }
            SetNatualAnimation();
        }
		if((_State == CharacterBase.eState.Idle || _State == CharacterBase.eState.Move) || _CurWeapon.isCancelable)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
				SwapWeapon(0);
			else if (Input.GetKeyDown(KeyCode.Alpha2))
				SwapWeapon(1);
			else if (Input.GetKeyDown(KeyCode.Alpha3))
				SwapWeapon(2);
			else if (Input.GetKeyDown(KeyCode.Alpha4))
				SwapWeapon(3);
			else if (Input.GetKeyDown(KeyCode.Alpha5))
				SwapWeapon(4);

			WeaponBase.eCommands Direction = WeaponBase.eCommands.None;
			if (Input.GetKey(KeyCode.A))
			{
				if (Mathf.Sign(transform.localScale.x) == -1)
					Direction = WeaponBase.eCommands.Front;
				else
					Direction = WeaponBase.eCommands.Back;
			}
			else if (Input.GetKey(KeyCode.D))
			{
				if (Mathf.Sign(transform.localScale.x) == 1)
					Direction = WeaponBase.eCommands.Front;
				else
					Direction = WeaponBase.eCommands.Back;
			}
			else if (Input.GetKey(KeyCode.W))
			{
				Direction = WeaponBase.eCommands.Up;
			}
			else if (Input.GetKey(KeyCode.S))
			{
				Direction = WeaponBase.eCommands.Down;
			}

			if (Input.GetKey(KeyCode.Mouse0))
				_CurWeapon.Attack(Direction, WeaponBase.eCommands.Left);
			else if (Input.GetKey(KeyCode.Mouse1))
				_CurWeapon.Attack(Direction, WeaponBase.eCommands.Right);
			else if (Input.GetKey(KeyCode.Mouse2))
				_CurWeapon.Attack(Direction, WeaponBase.eCommands.Middle);
		}
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            var contacts = collision.contacts;
            foreach (var contactPoint in contacts)
            {
                // 콜라이더의 밑 부분과 닿았는가??
                if (contactPoint.normal.y > 0)
                {
                    _CanJump = true;
                    _Animator.SetInteger(_AnimatorHash, Idle);
                
                    break;
                }
            }
        }
    }
    private void SetNatualAnimation()
    {
        if (_Rigidbody.velocity.y < 0)
        {
            _Animator.SetInteger(_AnimatorHash, Landing);
        }
        else if (_Rigidbody.velocity.y > 0)
        {
            _Animator.SetInteger(_AnimatorHash, Jump);
        }
    }
    public void MoveOrder(Vector2 direction, Func<bool> moveStop)
    {
        if (_MoveRoutine != null) {
            StopCoroutine(_MoveRoutine);
        }
        StartCoroutine(_MoveRoutine = MoveRoutine(direction, moveStop));
    }
    private IEnumerator MoveRoutine(Vector3 direction, Func<bool> moveStop)
    {
        if(_State == CharacterBase.eState.Idle || _State == CharacterBase.eState.Move)
			_State = CharacterBase.eState.Move;
        do
        {
			if (_State == CharacterBase.eState.Move)
			{
				Vector3 Scale = transform.localScale;
				Scale.x = Mathf.Sign(direction.x) * Mathf.Abs(Scale.x);
				transform.localScale = Scale;
				if (_Animator.GetInteger(_AnimatorHash) == Idle && _Rigidbody.velocity.y == 0)
				{
					_Animator.SetInteger(_AnimatorHash, Move);
				}
				_Rigidbody.AddForce(direction * _MoveSpeed * Time.deltaTime * Time.timeScale);
				{
					Vector2 velocity = _Rigidbody.velocity;

					_Rigidbody.velocity = new Vector2
						(Mathf.Clamp(velocity.x, -_MoveSpeedMax, _MoveSpeedMax), velocity.y);
				}
			}
			else
			{
				if (_Animator.GetInteger(_AnimatorHash) == Move && _Rigidbody.velocity.y == 0)
				{
					_Animator.SetInteger(_AnimatorHash, Idle);
				}
				_MoveRoutine = null;
				yield break;
			}
            yield return null;
        }
        while (!moveStop.Invoke());

        if (_Animator.GetInteger(_AnimatorHash) == Move) {
            _Animator.SetInteger(_AnimatorHash, Idle);
        }
        _MoveRoutine = null;
        // ========== Slip Routine ========== //
        float velX = _Rigidbody.velocity.x;

        for (float i = 0f; i < _SlipTime; i += Time.deltaTime * Time.timeScale)
        {
            float ratio = _SlipCurve.Evaluate(Mathf.Min(i / _SlipTime, 1f));

            Vector2 velocity = _Rigidbody.velocity;
            _Rigidbody.velocity = new Vector2(Mathf.Lerp(velX, 0f, ratio), velocity.y);

            yield return null;
        }
        // ========== Slip Routine ========== //
		if (_State == CharacterBase.eState.Move)
		{
			_State = CharacterBase.eState.Idle;
		}
    }
    public void HandleAnimationEventsToWeapon(WeaponBase.eWeaponEvents weaponEvent)
    {
        _CurWeapon.HandleAnimationEvents(weaponEvent);
    }
    private void InitWeapons()
    {
        _WeaponDatas[(int)WeaponBase.eWeapons.Glove] = new Wep_Glove(this, _Animator);
        _WeaponDatas[(int)WeaponBase.eWeapons.Sword] = new Wep_Sword(this, _Animator);
        _WeaponDatas[(int)WeaponBase.eWeapons.Akimbo_Pistol] = new Wep_Akimbo_Pistol(this, _Animator);

		_EqiupedWeapons[0] = WeaponBase.eWeapons.Glove;
		_EqiupedWeapons[1] = WeaponBase.eWeapons.Sword;
		_EqiupedWeapons[2] = WeaponBase.eWeapons.Akimbo_Pistol;
		_EqiupedWeapons[3] = WeaponBase.eWeapons.None;
		_EqiupedWeapons[4] = WeaponBase.eWeapons.None;

        _CurWeapon = _WeaponDatas[(int)WeaponBase.eWeapons.Sword];
    }
	private void SwapWeapon(int index)
	{
		if (_EqiupedWeapons[index] == WeaponBase.eWeapons.None)
			return;
		if (_CurWeapon == _WeaponDatas[(int)_EqiupedWeapons[index]])
			return;
		_CurWeapon = _WeaponDatas[(int)_EqiupedWeapons[index]];
		_CurWeapon.OnSwap();
		_State = CharacterBase.eState.Idle;
		NextAnimation = "Idle";
		_Animator.SetInteger(_AnimatorHash, Idle);
	}
    public void AddForceX(float x)
    {
        _Rigidbody.velocity = new Vector2(x * Mathf.Sign(transform.localScale.x), _Rigidbody.velocity.y);
    }
    public void AddForceY(float y)
    {
        _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, y);
    }
}

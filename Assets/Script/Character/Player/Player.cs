using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Animation Transition //
    private const int Idle    = 0;
    private const int Move    = 1;
    private const int Jump    = 2;
    private const int Landing = 3;

    [Header("Move Property")]
    [SerializeField] private Rigidbody2D _Rigidbody;
    public Rigidbody2D Rigidbody => _Rigidbody;

    [SerializeField] private float _JumpForce;
    private bool _CanJump;

    [SerializeField] private float _MoveSpeed;
    [SerializeField] private float _MoveSpeedMax;
    private IEnumerator _MoveRoutine;

    [Header("Slip Property")] // 이동이 끝난 후 미끄러지는거
    [SerializeField, Range(0f, 3f)] private float _SlipTime;
    [SerializeField] private AnimationCurve _SlipCurve;

    [Header("Other Property")]
    [SerializeField] private Animator _Animator;
    private int _AnimatorHash;

    public StateBase.eState State { get; set; }

    // 무기
    private WeaponBase _CurWeapon;
    private WeaponBase.eWeapons[] _EqiupedWeapons = new WeaponBase.eWeapons[5];
    private WeaponBase[] _WeaponDatas = new WeaponBase[(int)WeaponBase.eWeapons.End];

    private void Awake()
    {
        _CanJump = true;
        _AnimatorHash = _Animator.GetParameter(0).nameHash;
        State = StateBase.eState.Idle;
        InitWeapons();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveOrder(Vector2.left, () => Input.GetKeyUp(KeyCode.A));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveOrder(Vector2.right,() => Input.GetKeyUp(KeyCode.D));
        }

        if (State == StateBase.eState.Idle || State == StateBase.eState.Move)
        {
            if (Input.GetKeyDown(KeyCode.Space) && _CanJump)
            {
                _Rigidbody.AddForce(Vector2.up * _JumpForce, ForceMode2D.Impulse);
                _CanJump = false;
            }
            SetNatualAnimation();

            WeaponBase.eCommands Direction = WeaponBase.eCommands.None;

            if (Input.GetKey(KeyCode.A))
            {
                State = StateBase.eState.Move;
                if (Mathf.Sign(transform.localScale.x) == -1)
                    Direction = WeaponBase.eCommands.Front;
                else
                    Direction = WeaponBase.eCommands.Back;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                State = StateBase.eState.Move;
                if (Mathf.Sign(transform.localScale.x) == 1)
                    Direction = WeaponBase.eCommands.Front;
                else
                    Direction = WeaponBase.eCommands.Back;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                Direction = WeaponBase.eCommands.Up;
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
        if (State != StateBase.eState.Idle)
            return;

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
        if(State == StateBase.eState.Idle || State == StateBase.eState.Move)
            State = StateBase.eState.Move;
        do
        {
            if (State == StateBase.eState.Move)
            {
				Vector3 Scale = transform.localScale;
				Scale.x = Mathf.Sign(direction.x) * Mathf.Abs(Scale.x);
				transform.localScale = Scale;
                if (_Animator.GetInteger(_AnimatorHash) == Idle)
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
            yield return null;
        }
        while (!moveStop.Invoke());

        if (_Animator.GetInteger(_AnimatorHash) == Move) {
            _Animator.SetInteger(_AnimatorHash, Idle);
        }
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
        _MoveRoutine = null;
        if(State == StateBase.eState.Move)
          State = StateBase.eState.Idle;
    }
    public void HandleAnimationEventsToWeapon(WeaponBase.eWeaponEvents weaponEvent)
    {
        _CurWeapon.HandleAnimationEvents(weaponEvent);
    }
    private void InitWeapons()
    {
        _WeaponDatas[(int)WeaponBase.eWeapons.Glove] = new Wep_Glove(this, _Animator);

        _CurWeapon = _WeaponDatas[(int)WeaponBase.eWeapons.Glove];
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

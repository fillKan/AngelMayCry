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

    private void Awake()
    {
        _CanJump = true;
        _AnimatorHash = _Animator.GetParameter(0).nameHash;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _CanJump)
        {
            _Rigidbody.AddForce(Vector2.up * _JumpForce, ForceMode2D.Impulse);
            _CanJump = false;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveOrder(Vector2.left, () => Input.GetKeyUp(KeyCode.A));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveOrder(Vector2.right,() => Input.GetKeyUp(KeyCode.D));
        }
        SetNatualAnimation();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            _CanJump = true;
            _Animator.SetInteger(_AnimatorHash, Idle);
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
        else if(_Rigidbody.velocity.magnitude == 0f)
        {
            _Animator.SetInteger(_AnimatorHash, Idle);
        }
    }
    public void MoveOrder(Vector2 direction, Func<bool> moveStop)
    {
        transform.rotation = direction.x < 0 ? 
            Quaternion.identity : Quaternion.Euler(0, 180, 0);

        if (_MoveRoutine != null) {
            StopCoroutine(_MoveRoutine);
        }
        StartCoroutine(_MoveRoutine = MoveRoutine(direction, moveStop));
    }
    private IEnumerator MoveRoutine(Vector3 direction, Func<bool> moveStop)
    {
        do
        {
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
    }
}

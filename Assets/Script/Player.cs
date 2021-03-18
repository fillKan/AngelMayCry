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

    [SerializeField] private Rigidbody2D _Rigidbody;
    public Rigidbody2D Rigidbody => _Rigidbody;

    [SerializeField] private float _JumpForce;
    private bool _CanJump;

    [SerializeField] private float _MoveSpeed;
    private IEnumerator _MoveRoutine;

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
            _Rigidbody.AddForce(Vector2.up * _JumpForce);
            _CanJump = false;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveOrder(Vector2.left, KeyCode.A);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveOrder(Vector2.right, KeyCode.D);
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
    }
    public void MoveOrder(Vector2 direction, KeyCode keyCode)
    {
        transform.rotation = direction.x < 0 ? 
            Quaternion.identity : Quaternion.Euler(0, 180, 0);

        if (_MoveRoutine != null) {
            StopCoroutine(_MoveRoutine);
        }
        StartCoroutine(_MoveRoutine = MoveRoutine(direction, keyCode));
    }
    private IEnumerator MoveRoutine(Vector3 direction, KeyCode keyCode)
    {
        do
        {
            if (_Animator.GetInteger(_AnimatorHash) == Idle) {
                _Animator.SetInteger(_AnimatorHash, Move);
            }
            transform.position += direction * _MoveSpeed * Time.deltaTime * Time.timeScale;
            yield return null;
        }
        while (!Input.GetKeyUp(keyCode));

        _Animator.SetInteger(_AnimatorHash, Idle);
        SetNatualAnimation();

        _MoveRoutine = null;
    }
}

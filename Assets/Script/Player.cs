using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState
{
    Idle, Move, Jump, Landing
}
public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _Rigidbody;
    public Rigidbody2D Rigidbody => _Rigidbody;

    [SerializeField] private float _JumpForce;
    private bool _CanJump;

    [SerializeField] private float _MoveSpeed;

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

            _Animator.SetInteger(_AnimatorHash, (int)AnimState.Jump);
            Debug.Log((AnimState)_Animator.GetInteger(_AnimatorHash));
            _CanJump = false;
        }
        if (_Animator.GetInteger(_AnimatorHash) == (int)AnimState.Jump)
        {
            if (_Rigidbody.velocity.y < 0)
            {
                _Animator.SetInteger(_AnimatorHash, (int)AnimState.Landing);
                Debug.Log((AnimState)_Animator.GetInteger(_AnimatorHash));
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            _CanJump = true;
            _Animator.SetInteger(_AnimatorHash, (int)AnimState.Idle);
            Debug.Log((AnimState)_Animator.GetInteger(_AnimatorHash));
        }
    }
}

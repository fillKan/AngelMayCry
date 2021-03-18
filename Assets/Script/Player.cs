using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _Rigidbody;
    public Rigidbody2D Rigidbody => _Rigidbody;

    [SerializeField] private float _JumpForce;
    private bool _CanJump;

    private void Awake()
    {
        _CanJump = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _CanJump)
        {
            _CanJump = false;
            _Rigidbody.AddForce(Vector2.up * _JumpForce);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            _CanJump = true;
        }
    }
}

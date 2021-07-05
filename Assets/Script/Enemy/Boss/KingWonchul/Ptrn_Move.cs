using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ptrn_Move : BossPattern
{
    public override int AnimationCode => 9;
    public override bool CanAction => !_HasPlayer;

    [Header("Move Property")]
    [SerializeField] private Rigidbody2D _Rigidbody;
    [SerializeField] private float _StepForce;

    public override void Action()
    {
        base.Action();
        StartCoroutine(MoveRoutine());
    }
    private void AE_Move_Step()
    {
        _Rigidbody.AddForce(Vector2.left * _StepForce);
    }
    private IEnumerator MoveRoutine()
    {
        bool IsMoving() 
            => _Animator.GetInteger(_AnimatorHash) == AnimationCode;
        
        for (float i = 0f; i < 2f; i += Time.deltaTime * Time.timeScale)
        {
            if (!IsMoving())
            { yield break; }

            yield return null; 
        }
        AE_SetDefaultState();
    }
}

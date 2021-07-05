using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ptrn_Slash : BossPattern
{
    public override bool CanAction => _HasPlayer;

    public override int AnimationCode => _AnimationCode;
    private int _AnimationCode = 7;

    [Header("Step Property")]
    [SerializeField] private Rigidbody2D _Rigidbody;
    [SerializeField] private float _StepForce;

    public override void Action()
    {
        _AnimationCode = Random.Range(7, 8 + 1);
        base.Action();
    }
    private void AE_Slash_Step()
    {
        _Rigidbody.AddForce(Vector2.left * _StepForce);
    }
    private void AE_SlashInner_Shake()
    {
        MainCamera.Instance.CameraShake(0.4f, 0.2f);
    }
    private void AE_SlashOutter_Shake()
    {
        MainCamera.Instance.CameraShake(0.9f, 0.15f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ptrn_SlashSlash : BossPattern
{
    public override int AnimationCode => 2;
    public override bool CanAction => _HasPlayer;

    private void AE_SSlash_End()
    {
        _Animator.SetInteger(_AnimatorHash, _DefaultAnimationCode);
    }
    private void AE_SSlash_Shake1()
    {
        MainCamera.Instance.CameraShake(0.4f, 0.2f);
    }
    private void AE_SSlash_Shake2()
    {
        MainCamera.Instance.CameraShake(0.9f, 0.15f);
    }
}

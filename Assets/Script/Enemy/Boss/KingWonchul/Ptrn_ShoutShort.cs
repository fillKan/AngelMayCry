using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ptrn_ShoutShort : BossPattern
{
    public override int AnimationCode => 4;
    public override bool CanAction => _HasPlayer;

    [Header("Shouting Property")]
    [SerializeField] private ParticleSystem _ShoutingEffect;

	private void AE_ShoutShort_End()
	{
		MainCamera.Instance.CameraShake(0.8f, 0.25f);
		_ShoutingEffect.Play();
	}
}

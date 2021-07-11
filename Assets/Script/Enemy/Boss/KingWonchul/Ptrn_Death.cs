using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ptrn_Death : BossPattern
{
	public override int AnimationCode => 21;

	[Header("Effects Property")]
	[SerializeField] private ParticleSystem _ShoutingEffect;
	[SerializeField] private ParticleSystem _GhostEffect;

	public override void Action()
	{
		base.Action();
		_Animator.Play("TheKing_Wonchul_Death");
		foreach(var iter in GameObject.FindGameObjectsWithTag("Enemy"))
		{
			iter.GetComponent<CharacterBase>().Death();
		}
	}

	private void AE_Death_CameraShake_Rise()
	{
		MainCamera.Instance.CameraShake(2f, 0.35f, ShakeStyle.Rise);
	}

	private void AE_Death_Shout()
	{
		MainCamera.Instance.CameraShake(3f, 0.35f, ShakeStyle.Cliff);
		_ShoutingEffect.Play();
		_GhostEffect.Play();
	}

	private void AE_Death_Down()
	{
		MainCamera.Instance.CameraShake(2f, 0.1f);
	}
}

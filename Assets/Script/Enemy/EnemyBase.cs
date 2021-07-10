using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterBase
{
	[SerializeField] protected MovementModule _MovementModule;
	[SerializeField] protected SecondaryCollider _DetectionRange;

	protected bool _IsAlreadyInit = false;

	protected override void Awake()
	{
		base.Awake();
		if (!_IsAlreadyInit)
		{
			_OnIdle = () =>
			{
				_MovementModule.RoutineEnable = true;
			};

			_OnHit = () =>
			{
				_MovementModule.RoutineEnable = false;
				_MovementModule.TrackingStop();
				_MovementModule.MoveStop();
				_MovementModule.StopAllCoroutines();
			};
		}
	}
}

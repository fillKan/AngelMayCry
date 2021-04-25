using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterBase
{
	[SerializeField] protected MovementModule _MovementModule;
	[SerializeField] protected SecondaryCollider _DetectionRange;

	protected bool _IsAlreadyInit = false;

	protected virtual void Awake()
	{
		if (!_IsAlreadyInit)
		{
			_OnIdle = () =>
			{
				Debug.Log("Idle");
				_MovementModule.RoutineEnable = true;
			};

			_OnHit = () =>
			{
				Debug.Log("Hit");
				_MovementModule.RoutineEnable = false;
				_MovementModule.MoveStop();
				_MovementModule.StopAllCoroutines();
			};
		}
	}
}

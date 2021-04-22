using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonWonchul : EnemyBase
{
    private const int Idle = 0;
    private const int Move = 1;
    private const int Attack = 2;

    [SerializeField] private MovementModule _MovementModule;

    [SerializeField] private SecondaryCollider _DetectionRange;

    private bool _IsAlreadyInit = false;
    private int _AnimHash;

	private new void  OnEnable()
    {
		base.OnEnable();
        if (!_IsAlreadyInit)
        {
            _AnimHash = _Animator.GetParameter(0).nameHash;

            _MovementModule.BehaviourBegin += behaviour =>
            {
                _Animator.SetInteger(_AnimHash, Move);
            };
            _MovementModule.BehaviourEnd += behaviour =>
            {
                _Animator.SetInteger(_AnimHash, Idle);
            };
            _DetectionRange.OnTriggerAction = (other, enter) =>
            {
                if (other.CompareTag("Player"))
                {
                    if (enter)
                    {
                        _MovementModule.TrackingComplete = AttackRoutine();
                        _MovementModule.TrackingStart(other.transform);
                    }
                    else
                    {
                        _MovementModule.TrackingStop();
                    }
                }
            };
            _IsAlreadyInit = true;

            _MovementModule.Operation();
        }
    }

    private IEnumerator AttackRoutine()
    {
        _Animator.SetInteger(_AnimHash, Attack);
        for (float i = 0f; i < 0.9f; i += Time.deltaTime * Time.timeScale)
            yield return null;

        _Animator.SetInteger(_AnimHash, Idle);
        for (float i = 0f; i < 0.4f; i += Time.deltaTime * Time.timeScale)
            yield return null;

        _MovementModule.TrackingComplete = AttackRoutine();
    }
    private void AnimEvnt_Attack(int strokeCount)
    {
        switch (strokeCount)
        {
            case 1:
                MainCamera.Instance.CameraShake(0.15f, 0.03f);
                break;
            case 2:
                MainCamera.Instance.CameraShake(0.2f, 0.05f);
                break;
        }
    }
}

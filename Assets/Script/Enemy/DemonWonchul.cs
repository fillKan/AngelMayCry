using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonWonchul : MonoBehaviour
{
    private const int Idle = 0;
    private const int Move = 1;

    [SerializeField] private MovementModule _MovementModule;
    [SerializeField] private Animator _Animator;

    private bool _IsAlreadyInit = false;
    private int _AnimHash;

    private void OnEnable()
    {
        if (!_IsAlreadyInit)
        {
            _AnimHash = _Animator.GetParameter(0).nameHash;

            _MovementModule.MoveBeginAction += () =>
            {
                _Animator.SetInteger(_AnimHash, Move);
            };
            _MovementModule.MoveEndAction += () =>
            {
                _Animator.SetInteger(_AnimHash, Idle);
            };
            _IsAlreadyInit = true;
        }
    }
}

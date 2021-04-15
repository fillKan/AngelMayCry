using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonWonchul : MonoBehaviour
{
    [SerializeField] private MovementModule _MovementModule;

    [SerializeField] private SecondaryCollider _LeftBrake;
    [SerializeField] private SecondaryCollider _RightBrake;

    private bool _IsAlreadyInit = false;

    private void OnEnable()
    {
        if (!_IsAlreadyInit)
        {
            _LeftBrake.OnTriggerExit += o =>
            {
                if (o.CompareTag("Ground"))
                {
                    _MovementModule.MoveStop();
                    _MovementModule.NextMoveDirection = Vector2.right;
                }
            };
            _RightBrake.OnTriggerExit += o =>
            {
                if (o.CompareTag("Ground"))
                {
                    _MovementModule.MoveStop();
                    _MovementModule.NextMoveDirection = Vector2.left;
                }
            };
            _IsAlreadyInit = true;
        }
    }
}

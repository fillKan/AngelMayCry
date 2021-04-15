using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase : MonoBehaviour
{
    public enum eState
    {
        Idle,
        Move,
        Attack,
        Hit,
        Down,
        Wake,
        Dead,
        End
    }


}

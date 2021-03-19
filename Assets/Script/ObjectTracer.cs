using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTracer : MonoBehaviour
{
    public Transform TraceTarget;
    [SerializeField] private Vector3 _Offset;

    private void LateUpdate()
    {
        if (TraceTarget) {
            transform.localPosition = TraceTarget.position + _Offset;
        }
    }
}

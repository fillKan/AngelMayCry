using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementModule : MonoBehaviour
{
    private static readonly Quaternion LookAtRight = 
        Quaternion.Euler(0, 180, 0);

    public event System.Action MoveBeginAction;
    public event System.Action MoveEndAction;

    // =========== ============== =========== //
    // =========== Inspector Vlew =========== //
    public AnimatorUpdateMode TimeMode;
    public SlipingEffector Sliping;
    public Rigidbody2D Rigidbody;

    [Header("WaitTime Property")]
    [Range(0f, 2f)] public float WaitTimeMin;
    [Range(0f, 2f)] public float WaitTimeMax;

    [Header("MoveTime Property")]
    [Range(0f, 4f)] public float MoveTimeMin;
    [Range(0f, 4f)] public float MoveTimeMax;

    [Header("Movement Property")]
    public float MoveSpeed;
    public float MoveSpeedMax;
    // =========== Inspector Vlew =========== //
    // =========== ============== =========== //

    public bool RoutineEnable
    {
        get => _RoutineEnable;
        set
        {
            if (_RoutineEnable = value)
            {
                if (_MoveCycleRoutine == null)
                    StartCoroutine(_MoveCycleRoutine = MoveCycleRoutine());
            }
            else
            {
                if (_MoveRoutine != null)
                {
                    StopCoroutine(_MoveRoutine);
                    _MoveRoutine = null;
                }
                if (_MoveCycleRoutine != null)
                {
                    StopCoroutine(_MoveCycleRoutine);
                    _MoveCycleRoutine = null;
                }
            }
        }
    }
    private bool _RoutineEnable = true;

    private IEnumerator _MoveCycleRoutine = null;
    private IEnumerator _MoveRoutine = null;
    
    private void OnEnable()
    {
        RoutineEnable = _RoutineEnable;
    }
    private float DeltaTime()
    {
        switch (TimeMode)
        {
            case AnimatorUpdateMode.Normal:
                return Time.deltaTime;

            case AnimatorUpdateMode.AnimatePhysics:
                return Time.fixedDeltaTime;

            case AnimatorUpdateMode.UnscaledTime:
                return Time.unscaledTime;

            default: return Time.deltaTime;
        }
    }
    private IEnumerator MoveCycleRoutine()
    {
        while (RoutineEnable)
        {
            Vector2 dir = Random.value < 0.5f ? Vector2.left : Vector2.right;
            transform.rotation = dir.x > 0 ? LookAtRight : Quaternion.identity;

            float move = Random.Range(MoveTimeMin, MoveTimeMax);
            yield return StartCoroutine(MoveRoutine(dir, move));

            float wait = Random.Range(WaitTimeMin, WaitTimeMax);
            for (float i = 0; i < wait; i += DeltaTime())
                yield return null;
        }
    }
    private IEnumerator MoveRoutine(Vector2 direction, float moveTime)
    {
        MoveBeginAction?.Invoke();

        for (float i = 0; i < moveTime; i += DeltaTime())
        {
            Rigidbody.AddForce(direction * MoveSpeed * DeltaTime());
            Vector2 vel = Rigidbody.velocity;

            Rigidbody.velocity = 
                new Vector2(Mathf.Clamp(vel.x, -MoveSpeedMax, MoveSpeedMax), vel.y);

            yield return null;
        }
        MoveEndAction?.Invoke();

        Sliping.Start();
        while (Sliping.IsProceeding)
            yield return null;

        yield break;
    }
}

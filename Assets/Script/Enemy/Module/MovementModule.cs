using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementModule : MonoBehaviour
{
    public event System.Action MoveBeginAction;
    public event System.Action MoveEndAction;

    public event System.Action TrackingEndAction;

    // =========== ============== =========== //
    // =========== Inspector Vlew =========== //
    public AnimatorUpdateMode TimeMode;
    public SlipingEffector Sliping;
    public Rigidbody2D Rigidbody;
    public SecondaryCollider FrontBrake;

    [Header("WaitTime Property")]
    [Range(0f, 2f)] public float WaitTimeMin;
    [Range(0f, 2f)] public float WaitTimeMax;

    [Header("MoveTime Property")]
    [Range(0f, 4f)] public float MoveTimeMin;
    [Range(0f, 4f)] public float MoveTimeMax;

    [Header("Movement Property")]
    public float MoveSpeed;
    public float MoveSpeedMax;

    [Header("Tracing Property")]
    public float TrackingDistance;
    // =========== Inspector Vlew =========== //
    // =========== ============== =========== //

    [HideInInspector] public Vector2 NextMoveDirection = Vector2.zero;
    [HideInInspector] public Vector2 LastMoveDirection = Vector2.zero;

    [HideInInspector] public Transform TrackingTarget;

    public IEnumerator TrackingComplete;
    private IEnumerator _TrackingRoutine = null;

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

    public void Operation()
    {
        RoutineEnable = _RoutineEnable;

        FrontBrake.OnTriggerAction = (o, enter) =>
        {
            if (o.CompareTag("Ground") && !enter) {
                MoveStop();

                NextMoveDirection = -LastMoveDirection;
            }
        };
    }
    public void TrackingStart(Transform target)
    {
        TrackingTarget = target;

        if (_TrackingRoutine != null)
            StopCoroutine(_TrackingRoutine);

        _TrackingRoutine = TrackingRoutine();
        MoveStop();
    }
    public void TrackingStop()
    {
        if (_TrackingRoutine != null)
            StopCoroutine(_TrackingRoutine);

        _TrackingRoutine = null;
        TrackingComplete = null;
        TrackingTarget = null;
    }
    public void MoveStop()
    {
        if (_MoveRoutine!=null)
            StopCoroutine(_MoveRoutine);

        _MoveRoutine = null;
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
            if (TrackingTarget != null)
            {
                StartCoroutine(_TrackingRoutine = TrackingRoutine());

                while (_TrackingRoutine != null)
                    yield return null;
                Sliping.Start();

                yield return TrackingComplete;
                TrackingEndAction?.Invoke();

                while (Sliping.IsProceeding)
                    yield return null;
            }
            else
            {
                Vector2 dir;
                if (Mathf.Abs(NextMoveDirection.x) > 0)
                {
                    dir = NextMoveDirection;
                }
                else
                    dir = Random.value < 0.5f ? Vector2.left : Vector2.right;

                Vector2 scale = transform.localScale;
                scale.x = dir.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                transform.localScale = scale;

                LastMoveDirection = dir;
                NextMoveDirection = Vector2.zero;

                float move = Random.Range(MoveTimeMin, MoveTimeMax);
                StartCoroutine(_MoveRoutine = MoveRoutine(dir, move));

                while (_MoveRoutine != null)
                    yield return null;
                MoveEndAction?.Invoke();

                Sliping.Start();
                while (Sliping.IsProceeding)
                    yield return null;
            }
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
        _MoveRoutine = null;
    }
    private IEnumerator TrackingRoutine()
    {
        var tracerX = transform.position.x;
        var targetX = TrackingTarget.position.x;

        if (Mathf.Abs(targetX - tracerX) > TrackingDistance)
        {
            MoveBeginAction?.Invoke();
        }
        while (TrackingTarget != null)
        {
            tracerX = transform.position.x;
            targetX = TrackingTarget.position.x;

            if (Mathf.Abs(targetX - tracerX) <= TrackingDistance)
            {
                break;
            }
            var direction = new Vector2(Mathf.Sign(targetX - tracerX), 0);

            Rigidbody.AddForce(direction * MoveSpeed * DeltaTime());
            Vector2 vel = Rigidbody.velocity;

            Rigidbody.velocity =
                new Vector2(Mathf.Clamp(vel.x, -MoveSpeedMax, MoveSpeedMax), vel.y);

            yield return null;
        }
        _TrackingRoutine = null;
    }
}

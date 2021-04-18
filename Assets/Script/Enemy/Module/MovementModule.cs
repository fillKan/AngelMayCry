using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementModule : MonoBehaviour
{
    public enum BehaviourIndex
    {
        Default, Tracking
    }
    public event System.Action<BehaviourIndex> BehaviourBegin;
    public event System.Action<BehaviourIndex> BehaviourEnd;

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

    [Header("Tracking Property")]
    public float TrackingDistance;
    // =========== Inspector Vlew =========== //
    // =========== ============== =========== //

    [HideInInspector] public Vector2 NextMoveDirection = Vector2.zero;
    [HideInInspector] public Vector2 LastMoveDirection = Vector2.zero;

    [HideInInspector] public Transform TrackingTarget;

    public IEnumerator TrackingComplete;
    private IEnumerator _TrackingBehaviour = null;

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
                if (_MoveBehaviour != null)
                {
                    StopCoroutine(_MoveBehaviour);
                    _MoveBehaviour = null;
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
    private IEnumerator _MoveBehaviour = null;

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

        if (_TrackingBehaviour != null)
            StopCoroutine(_TrackingBehaviour);

        _TrackingBehaviour = TrackingBehaviour();
        MoveStop();
    }
    public void TrackingStop()
    {
        if (_TrackingBehaviour != null)
            StopCoroutine(_TrackingBehaviour);

        TrackingComplete = null;
        TrackingTarget = null;
        _TrackingBehaviour = null;
    }
    public void MoveStop()
    {
        if (_MoveBehaviour!=null)
            StopCoroutine(_MoveBehaviour);

        _MoveBehaviour = null;
    }
    public BehaviourIndex GetBehaviourState()
    {
        if (TrackingTarget != null)
        {
            return BehaviourIndex.Tracking;
        }
        return BehaviourIndex.Default;
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
            BehaviourIndex behaviour = GetBehaviourState();

            switch (behaviour)
            {
                case BehaviourIndex.Tracking:
                    yield return StartCoroutine(TrackingRoutine());
                    break;

                case BehaviourIndex.Default:
                    yield return StartCoroutine(MoveRoutine());
                    break;
            }            
            float wait = Random.Range(WaitTimeMin, WaitTimeMax);
            for (float i = 0; i < wait; i += DeltaTime())
            {
                if (GetBehaviourState() == BehaviourIndex.Tracking)
                    break;

                yield return null;
            }
        }
    }
    // ========== Move ========== //
    private IEnumerator MoveRoutine()
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
        StartCoroutine(_MoveBehaviour = MoveBehaviour(dir, move));

        while (_MoveBehaviour != null)
            yield return null;
        BehaviourEnd?.Invoke(BehaviourIndex.Default);

        Sliping.Start();
        while (Sliping.IsProceeding)
            yield return null;
    }
    private IEnumerator MoveBehaviour(Vector2 direction, float moveTime)
    {
        BehaviourBegin?.Invoke(BehaviourIndex.Default);

        for (float i = 0; i < moveTime; i += DeltaTime())
        {
            Rigidbody.AddForce(direction * MoveSpeed * DeltaTime());
            Vector2 vel = Rigidbody.velocity;

            Rigidbody.velocity = 
                new Vector2(Mathf.Clamp(vel.x, -MoveSpeedMax, MoveSpeedMax), vel.y);

            yield return null;
        }
        _MoveBehaviour = null;
    }
    // ========== Move ========== //

    // ========== Tracking ========== //
    private IEnumerator TrackingRoutine()
    {
        StartCoroutine(_TrackingBehaviour = TrackingBehaviour());

        while (_TrackingBehaviour != null)
            yield return null;
        Sliping.Start();

        yield return TrackingComplete;
        BehaviourEnd?.Invoke(BehaviourIndex.Tracking);

        while (Sliping.IsProceeding)
            yield return null;
    }
    private IEnumerator TrackingBehaviour()
    {
        var tracerX = transform.position.x;
        var targetX = TrackingTarget.position.x;

        if (Mathf.Abs(targetX - tracerX) > TrackingDistance)
        {
            BehaviourBegin?.Invoke(BehaviourIndex.Tracking);
        }
        while (TrackingTarget != null)
        {
            var direction = new Vector2(Mathf.Sign(targetX - tracerX), 0);

            Rigidbody.AddForce(direction * MoveSpeed * DeltaTime());
            Vector2 vel = Rigidbody.velocity;

            Rigidbody.velocity =
                new Vector2(Mathf.Clamp(vel.x, -MoveSpeedMax, MoveSpeedMax), vel.y);

            tracerX = transform.position.x;
            targetX = TrackingTarget.position.x;
            
            if (Mathf.Abs(targetX - tracerX) <= TrackingDistance) {
                break;
            }
            yield return null;
        }
        _TrackingBehaviour = null;
    }
    // ========== Tracking ========== //
}

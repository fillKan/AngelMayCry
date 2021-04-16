using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementModule : MonoBehaviour
{
    public event System.Action MoveBeginAction;
    public event System.Action MoveEndAction;

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
    // =========== Inspector Vlew =========== //
    // =========== ============== =========== //

    [HideInInspector] public Vector2 NextMoveDirection = Vector2.zero;
    [HideInInspector] public Vector2 LastMoveDirection = Vector2.zero;

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

        FrontBrake.OnTriggerExit += o =>
        {
            if (o.CompareTag("Ground")) {
                MoveStop();

                NextMoveDirection = -LastMoveDirection;
            }
        };
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
            Vector2 dir;
            if (Mathf.Abs(NextMoveDirection.x) > 0) {
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
}

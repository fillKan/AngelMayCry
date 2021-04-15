using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipingEffector : MonoBehaviour
{
    [Range(0f, 4f)] public float SlipingTime;

    public AnimationCurve SlipingCurve;
    public AnimatorUpdateMode UpdateMode;
    public Rigidbody2D Rigidbody;

    public bool IsProceeding
    { get => _SlipingRoutine != null; }

    private IEnumerator _SlipingRoutine = null;

    private void Reset() {
        TryGetComponent(out Rigidbody);
    }
    public void Stop()
    {
        if (_SlipingRoutine != null) {
            StopCoroutine(_SlipingRoutine);
        }
    }
    public void Start()
    {
        if (_SlipingRoutine != null) {
            StopCoroutine(_SlipingRoutine);
        } 
        StartCoroutine(_SlipingRoutine = SlipingRoutine());
    }

    private IEnumerator SlipingRoutine()
    {
        float DeltaTime()
        {
            switch (UpdateMode)
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
        float beginVelX = Rigidbody.velocity.x;

        for (float i = 0f; i < SlipingTime; i += DeltaTime())
        {
            float value = SlipingCurve.Evaluate(Mathf.Min(i / SlipingTime, 1f));
            
            float velX = Mathf.Lerp(beginVelX, 0, value);
            float velY = Rigidbody.velocity.y;

            Rigidbody.velocity = new Vector2(velX, velY);
            
            yield return null;
        }
        _SlipingRoutine = null;
    }
}
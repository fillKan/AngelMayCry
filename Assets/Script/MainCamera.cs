using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : Singleton<MainCamera>
{
    [SerializeField] private Camera _Camera;
    [SerializeField] private AnimationCurve _ShakeCurve;

    private float _RestShakeTime;
    private float _ShakeTime;
    private float _ShakeForcePerFrame;

    public void CameraShake(float time, float force)
    {
        float forcePerFrame = force;
        float ratio = 1f - Mathf.Min(_RestShakeTime / _ShakeTime, 1f);

        if (forcePerFrame > _ShakeForcePerFrame * _ShakeCurve.Evaluate(ratio))
        {
            _ShakeForcePerFrame = forcePerFrame;
            _RestShakeTime = _ShakeTime = time;
        }
        else
        {
            _ShakeForcePerFrame += forcePerFrame;
        }
    }
    private void Update()
    {
        if (_RestShakeTime > 0)
        {
            _RestShakeTime -= Time.unscaledDeltaTime;

            float ratio = 1f - _RestShakeTime / _ShakeTime;
            transform.position = Random.onUnitSphere * _ShakeForcePerFrame * _ShakeCurve.Evaluate(ratio);

            if (_RestShakeTime <= 0f) {
                _RestShakeTime = _ShakeForcePerFrame = _ShakeTime = 0f;
            }
        }
    }
}

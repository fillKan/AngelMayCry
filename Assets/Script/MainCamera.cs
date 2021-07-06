using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCamera : Singleton<MainCamera>
{
    [SerializeField] private Camera _Camera;
    [SerializeField] private AnimationCurve _ShakeCurve;
	[SerializeField] private Image _FadeMask;

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
	public void Fade(Color start, Color end, float time = 1, System.Action callback = null)
	{
		StartCoroutine(FadeRoutine(start, end, time, callback));
	}
	private IEnumerator FadeRoutine(Color start, Color end, float time, System.Action callback = null)
	{
		float curTime = 0;
		while (curTime < 1)
		{
			_FadeMask.color = Color.Lerp(start, end, curTime);
			curTime += 1f / time * Time.deltaTime;
			yield return null;
		}
		callback?.Invoke();
	}
	private void Start()
	{
		Fade(Color.black, new Color(0, 0, 0, 0));
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

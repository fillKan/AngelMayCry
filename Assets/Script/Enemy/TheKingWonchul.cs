using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheKingWonchul : MonoBehaviour
{
    private const int Idle = 0;
    private const int Appears = 1;
    private const int Slash = 2;
    private const int Groggy = 3;
    private const int Shouting = 4;
    private const int Ghost = 5;

    private const float FrameTime = 0.083f;
    [SerializeField, Range(0f, 1f)] private float _SlowScale;

    [SerializeField] private Animator _Animator;
    private int _AnimControlKey;

    [SerializeField] private float _PatternWait;
    [SerializeField] private float _GroggyTime;
    private void Awake()
    {
        _AnimControlKey = _Animator.GetParameter(0).nameHash;
    }
    [ContextMenu("GroggyOrder")]
    private void GroggyOrder()
    {
        _Animator.SetInteger(_AnimControlKey, Groggy);
    }
    [ContextMenu("ShoutingOrder")]
    private void ShoutingOrder()
    {
        _Animator.SetInteger(_AnimControlKey, Shouting);
    }
    private IEnumerator PatternTimer()
    {
        while (gameObject.activeSelf)
        {
            for (float i = 0f; i < _PatternWait; i += Time.deltaTime * Time.timeScale)
                yield return null;

            _Animator.SetInteger(_AnimControlKey, Slash);

            while (_Animator.GetInteger(_AnimControlKey) != Idle)
                yield return null;
        }
    }

    private void AE_Appears_SlowTime()
    {
        StartCoroutine(Appears_SlowTime(_SlowScale));
    }
    private void AE_Appears_Strike()
    {
        MainCamera.Instance.CameraShake(0.27f, 0.12f);
    }
    private void AE_Appears_End()
    {
        StartCoroutine(PatternTimer());
        _Animator.SetInteger(_AnimControlKey, Idle);
    }

    private void AE_Slash_End()
    {
        _Animator.SetInteger(_AnimControlKey, Idle);
    }
    private void AE_Slash_Shake1()
    {
        MainCamera.Instance.CameraShake(0.4f, 0.2f);
    }
    private void AE_Slash_Shake2()
    {
        MainCamera.Instance.CameraShake(0.9f, 0.15f);
    }

    private void AE_Groggy_LoopBegin()
    {
        MainCamera.Instance.CameraShake(1f, 0.2f);
        StartCoroutine(Groggy_Looping());
    }
    private IEnumerator Appears_SlowTime(float scale)
    {
        float time = FrameTime * 3.5f;
        for (float i = 0f; i < time; i += Time.deltaTime * Time.timeScale)
        {
            Time.timeScale = Mathf.Lerp(1f, scale, i / time);
            yield return null;
        }
        time = FrameTime * 2;
        for (float i = 0f; i < 0.15f; i += Time.deltaTime * Time.timeScale)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, i / time);
            yield return null;
        }
    }
    private IEnumerator Groggy_Looping()
    {
        for (float i = 0f; i < _GroggyTime; i += Time.deltaTime * Time.timeScale)
            yield return null;
        _Animator.SetInteger(_AnimControlKey, Idle);
    }
}

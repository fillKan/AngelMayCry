using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheKingWonchul : MonoBehaviour
{
    private const int Idle = 0;
    private const int Appears = 1;
    private const int SlashSlash = 2;
    private const int Groggy = 3;
    private const int Shouting = 4;
    private const int Shouting_Long = 5;
    private const int Ghost = 6;
    private const int Slash_Inner = 7;
    private const int Slash_Outter = 8;

    private const float Shouting_Duration = 3.3f;

    private const float FrameTime = 0.083f;
    [SerializeField, Range(0f, 1f)] private float _SlowScale;

    [SerializeField] private Animator _Animator;
    private int _AnimControlKey;

    [SerializeField] private float _PatternWait;
    [SerializeField] private float _GroggyTime;

    [Space()]
    [SerializeField] private BossPattern _ShoutingShort;
    [SerializeField] private BossPattern _ShoutingLong;

    private void Awake()
    {
        _AnimControlKey = _Animator.GetParameter(0).nameHash;

        _ShoutingShort.Init();
        _ShoutingLong.Init();
    }
    [ContextMenu("GroggyOrder")]
    private void GroggyOrder()
    {
        _Animator.SetInteger(_AnimControlKey, Groggy);
    }

    private IEnumerator PatternTimer()
    {
        while (gameObject.activeSelf)
        {
            for (float i = 0f; i < _PatternWait; i += Time.deltaTime * Time.timeScale)
                yield return null;

            switch (Random.Range(0,2))
            {
                case 0:
                    _ShoutingShort.Action();
                    break;
                case 1:
                    _ShoutingLong.Action();
                    break;
            }
            while (_Animator.GetInteger(_AnimControlKey) != Idle)
                yield return null;
        }
    }

    private void AE_SetIdleState()
    {
        _Animator.SetInteger(_AnimControlKey, Idle);
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
    private IEnumerator Shouting_Holding(float time)
    {
        for (float i = 0f; i < time; i += Time.deltaTime * Time.timeScale)
            yield return null;
        _Animator.SetInteger(_AnimControlKey, Idle);
    }
}

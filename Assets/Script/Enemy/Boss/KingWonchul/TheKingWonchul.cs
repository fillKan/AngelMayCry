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

    [Space()]
    [SerializeField] private BossPattern _Appears;
    [SerializeField] private BossPattern _Groggy;
    [SerializeField] private BossPattern _ShoutingShort;
    [SerializeField] private BossPattern _ShoutingLong;

    private void Awake()
    {
        _AnimControlKey = _Animator.GetParameter(0).nameHash;

        _Appears.Init();
        _Groggy.Init();
        _ShoutingShort.Init();
        _ShoutingLong.Init();

        _Appears.Action();
    }
    public void Awaken()
    {
        _ShoutingLong.Action();
        StartCoroutine(PatternTimer());
    }
    [ContextMenu("GroggyOrder")]
    private void GroggyOrder()
    {
        _Groggy.Action();
    }

    private IEnumerator PatternTimer()
    {
        while (_Animator.GetInteger(_AnimControlKey) != Idle)
            yield return null;

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
}

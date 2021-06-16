using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheKingWonchul : MonoBehaviour
{
    private const int Idle = 0;
    private const int Appears = 1;

    private const float FrameTime = 0.083f;
    [SerializeField, Range(0f, 1f)] private float _SlowScale;

    [SerializeField] private Animator _Animator;
    private int _AnimControlKey;

    private void Awake()
    {
        _AnimControlKey = _Animator.GetParameter(0).nameHash;
    }
    [ContextMenu("Appears Order")]
    private void AppearsOrder()
    {
        _Animator.SetInteger(_AnimControlKey, Appears);
    }
    private void AE_Appears_SlowTime()
    {
        StartCoroutine(Appears_SlowTime(_SlowScale));
    }
    private void AE_Appears_Strike()
    {
        MainCamera.Instance.CameraShake(0.27f, 0.12f);
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
}

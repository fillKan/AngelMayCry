using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ptrn_ShoutLong : BossPattern
{
    public override int AnimationCode => 5;

    private const float AnimHoldingTime = 2.6f;

    [Header("Shouting Property")]
    [SerializeField] private ParticleSystem _ShoutingEffect;
    [SerializeField] private CircleCollider2D _ShoutingHitCir;

    private void AE_ShoutLong_End()
    {
        StartCoroutine(PlayEffect());
        StartCoroutine(AnimHolding());
    }
    private IEnumerator PlayEffect()
    {
        _ShoutingHitCir.gameObject.SetActive(true);
        _ShoutingEffect.Play();

        float duration = _ShoutingEffect.main.duration + 0.3f;

        MainCamera.Instance.CameraShake(duration, (duration - 0.3f) * 0.1f, ShakeStyle.Cliff);
        for (float i = 0f; i < duration; i += Time.deltaTime * Time.timeScale)
        {
            _ShoutingHitCir.radius = Mathf.Lerp(0.5f, 10f, Mathf.Min(i / duration, 1f));
            yield return null;
        }
        _ShoutingHitCir.gameObject.SetActive(false);
    }
    private IEnumerator AnimHolding()
    {
        for (float i = 0f; i < AnimHoldingTime; i += Time.deltaTime * Time.timeScale)
            yield return null;
        _Animator.SetInteger(_AnimatorHash, _DefaultAnimationCode);
    }
}

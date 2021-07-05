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

    [Header("Summon Wonchul")]
    [SerializeField] private GameObject _Wonchul;
    [SerializeField] private float _Interval;
    [SerializeField] private float _SummonRangeWidth;

    private int _ShoutingCount = -1;

    public override void Action()
    {
        base.Action();
        _ShoutingCount++;
    }
    private void AE_ShoutLong_End()
    {
        StartCoroutine(PlayEffect());
        StartCoroutine(AnimHolding());
        StartCoroutine(SummonWonchul());
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
    private IEnumerator SummonWonchul()
    {
        Vector2 position = transform.position;

        for (int i = 0; i < _ShoutingCount * 3; i++)
        {
            for (float j = 0f; j < _Interval; j += Time.deltaTime * Time.timeScale)
            {
                if (_Animator.GetInteger(_AnimatorHash) != AnimationCode)
                    yield break;

                yield return null;
            }
            Vector2 summonPoint = position + new Vector2(Random.Range(-1f, 1f) * _SummonRangeWidth, -1.15f);
            Instantiate(_Wonchul, summonPoint, Quaternion.identity);
        }
    }
    private IEnumerator AnimHolding()
    {
        for (float i = 0f; i < AnimHoldingTime; i += Time.deltaTime * Time.timeScale)
            yield return null;
        _Animator.SetInteger(_AnimatorHash, _DefaultAnimationCode);
    }
}

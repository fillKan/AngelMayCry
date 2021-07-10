using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ptrn_ShoutLong : BossPattern
{
    public override int AnimationCode => 5;

    private const int GroggyState = 3;
    private const float AnimHoldingTime = 2.9f;

    [Header("Owner Property")]
    [SerializeField] private TheKingWonchul _Owner;
    [SerializeField] private Collider2D _HurtBox;

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
        if (_Owner.SuperArmor <= 0f)
        {
            StartCoroutine(ActionHolding());
        }
        else
        {
            base.Action();
        }
    }
    public override void Notify_HealthUpdate(float restPercent)
    {
        base.Notify_HealthUpdate(restPercent);

        // 0.7, 0.4, 0.1
        if (restPercent <= 1 - 0.3f * (_ShoutingCount + 1))
        {
            ++_ShoutingCount;
            Action();
        }
    }
    private void AE_ShoutLong_End()
    {
        StartCoroutine(PlayEffect());
        StartCoroutine(AnimDuration());
        StartCoroutine(SummonWonchul());
    }
    private IEnumerator PlayEffect()
    {
        _ShoutingHitCir.gameObject.SetActive(true);
        _ShoutingEffect.Play();

        float duration = _ShoutingEffect.main.duration + _ShoutingEffect.main.startLifetime.constant;

        MainCamera.Instance.CameraShake(duration, (duration - 0.3f) * 0.2f, ShakeStyle.Cliff);
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
    private IEnumerator AnimDuration()
    {
        for (float i = 0f; i < AnimHoldingTime; i += Time.deltaTime * Time.timeScale)
            yield return null;

        AE_SetDefaultState();
        _HurtBox.enabled = true;
    }
    private IEnumerator ActionHolding()
    {
        while (_Animator.GetInteger(_AnimatorHash) != _DefaultAnimationCode)
            yield return null;
        base.Action();
    }
}

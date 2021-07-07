using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ptrn_ShoutShort : BossPattern
{
    public override int AnimationCode => 4;
    public override bool CanAction => _HasPlayer;

    [Header("Shouting Property")]
    [SerializeField] private ParticleSystem _ShoutingEffect;
    [SerializeField] private CircleCollider2D _ShoutingHitCir;

    private void AE_ShoutShort_End()
    {
        MainCamera.Instance.CameraShake(0.8f, 0.25f);
        StartCoroutine(PlayEffect());
    }

    private IEnumerator PlayEffect()
    {
        _ShoutingHitCir.gameObject.SetActive(true);
        _ShoutingEffect.Play();

        float duration = _ShoutingEffect.main.duration;

        for (float i = 0f; i < duration; i += Time.deltaTime * Time.timeScale)
        {
            _ShoutingHitCir.radius = Mathf.Lerp(0.5f, 10f, Mathf.Min(i / duration, 1f));
            yield return null;
        }
        _ShoutingHitCir.gameObject.SetActive(false);
    }
}

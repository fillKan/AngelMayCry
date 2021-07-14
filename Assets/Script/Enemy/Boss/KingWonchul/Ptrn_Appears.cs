using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ptrn_Appears : BossPattern
{
    public override int AnimationCode => 1;

    [Header("Owner Property")]
    [SerializeField] private TheKingWonchul _Owner;

    [Header("Appears Action Property")]
    [SerializeField] private SlipingEffector _SlipingEffector;
    [SerializeField] private Rigidbody2D _Rigidbody;
    [SerializeField] private float _DashForce;

    [Header("Slow Time Property")]
    [SerializeField] private AnimationCurve _Curve;
    [SerializeField] private float _CurveTime;

    public override void Action()
    {
        base.Action();
        _Rigidbody.AddForce(new Vector2(-_DashForce, 0));
    }
    private void AE_Appears_StartTimeCurve()
    {
        StartCoroutine(TimeCurveRoutine(_CurveTime));
    }
    private void AE_Appears_Strike()
    {
        MainCamera.Instance.CameraShake(0.9f, 0.55f);
        _SlipingEffector.SlipingStart();
    }
    private void AE_Appears_End()
    {
        _Owner.Awaken();
    }

    private IEnumerator TimeCurveRoutine(float time)
    {
        for (float i = 0f; i < time; i += Time.deltaTime * Time.timeScale)
        {
            Time.timeScale = _Curve.Evaluate(Mathf.Min(1f, i / time));
            yield return null;
        }
        Time.timeScale = 1f;
    }
}

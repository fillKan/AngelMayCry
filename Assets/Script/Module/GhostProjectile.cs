using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostProjectile : MonoBehaviour
{
    private const float ShootingTime = 2f;

    public event System.Action<GhostProjectile> ReleaseEvent;

    [SerializeField]
    private Vector2 _PointB, _PointC;
    private Vector2 __PointA, __PointB, __PointC, __PointD;

    [Space()]
    [SerializeField] private float _PointB_Offset;
    [SerializeField] private float _PointC_Offset;
    [SerializeField] private float _PointD_Offset;

    [Space(), SerializeField] private float _Speed;

    public void Project(Transform target)
    {
        gameObject.SetActive(true);

        _Speed = Random.Range(1.2f, 0.8f);

        Vector2 start = transform.position;

        __PointA = start;
        __PointB = start + _PointB + Random.insideUnitCircle * _PointB_Offset;
        __PointC = start + _PointC + Random.insideUnitCircle * _PointC_Offset;

        __PointD = (Vector2)target.position + 
            Vector2.right * Random.Range(-1f, 1f) * _PointD_Offset;

        StartCoroutine(ProjectRoutine());
    }
    private IEnumerator ProjectRoutine()
    {
        for (float i = 0f; i < ShootingTime; i += Time.deltaTime * Time.timeScale * _Speed)
        {
            transform.localPosition = CaculateCurve(Mathf.Min(1f, i / ShootingTime));
            yield return null;
        }
        ReleaseEvent?.Invoke(this);
        MainCamera.Instance.CameraShake(0.2f, 0.15f);
    }
    private Vector3 CaculateCurve(float rate)
    {
        Vector2 a2b = Vector2.Lerp(__PointA, __PointB, rate);
        Vector2 b2c = Vector2.Lerp(__PointB, __PointC, rate);
        Vector2 c2d = Vector2.Lerp(__PointC, __PointD, rate);

        return Vector2.Lerp(Vector2.Lerp(a2b, b2c, rate), Vector2.Lerp(b2c, c2d, rate), rate);
    }
}
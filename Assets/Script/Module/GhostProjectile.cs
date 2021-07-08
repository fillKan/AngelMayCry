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

    [Header("Object Graghics")]
    [SerializeField] private SpriteRenderer _Renderer;
    [SerializeField] private ParticleSystem _ReleaseEffect;
    [SerializeField] private ParticleSystem _PathEffect;

	private CircleCollider2D _Collider;
    private Transform _Target;

    private Vector2 _LastTargetPoint;

	public void Awake()
	{
		_Collider = GetComponent<CircleCollider2D>();
	}

	public void Project(Transform target)
    {
        gameObject.SetActive(true);
        _PathEffect.Play();

        _Speed = Random.Range(1.2f, 0.8f);

        Vector2 start = transform.position;

        __PointA = start;
        __PointB = start + _PointB + Random.insideUnitCircle * _PointB_Offset;
        __PointC = start + _PointC + Random.insideUnitCircle * _PointC_Offset;

        _Target = target;
        _LastTargetPoint = _Target.position;
        __PointD = _LastTargetPoint + Vector2.right * Random.Range(-1f, 1f) * _PointD_Offset;

        _Renderer.enabled = true;
		_Collider.enabled = true;
		StartCoroutine(ProjectRoutine());
    }
    private IEnumerator ProjectRoutine()
    {
        int reCacluateCount = 1;

        for (float i = 0f; i < ShootingTime; i += Time.deltaTime * Time.timeScale * _Speed)
        {
            if (i >= reCacluateCount * 0.5f)
            {
                Vector2 nowPosition = _Target.position;
                Vector2 between = (nowPosition - _LastTargetPoint);
                reCacluateCount++;

                __PointD += between;
                _LastTargetPoint = nowPosition;
            }
            transform.localPosition = CaculateCurve(Mathf.Min(1f, i / ShootingTime));
            yield return null;
        }
        ReleaseEvent?.Invoke(this);
        MainCamera.Instance.CameraShake(0.2f, 0.15f);

        _ReleaseEffect.Play();
		_Collider.enabled = false;
        _Renderer.enabled = false;

		_PathEffect.Stop();
    }
    private Vector3 CaculateCurve(float rate)
    {
        Vector2 a2b = Vector2.Lerp(__PointA, __PointB, rate);
        Vector2 b2c = Vector2.Lerp(__PointB, __PointC, rate);
        Vector2 c2d = Vector2.Lerp(__PointC, __PointD, rate);

        return Vector2.Lerp(Vector2.Lerp(a2b, b2c, rate), Vector2.Lerp(b2c, c2d, rate), rate);
    }
}
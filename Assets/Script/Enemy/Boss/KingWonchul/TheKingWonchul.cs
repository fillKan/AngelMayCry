using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheKingWonchul : MonoBehaviour
{
    private readonly Vector3 LookLeft = Vector3.one;
    private readonly Vector3 LookRight = new Vector3(-1, 1, 1);

    private const int Idle = 0;

    [SerializeField] private Transform _HeartPoint;
    [SerializeField] private Animator _Animator;
    private int _AnimControlKey;

    [SerializeField] private float _PatternWait;

    [Header("Recognition")]
    [SerializeField] private Transform _PlayerTransform;
    [SerializeField] private SecondaryCollider _RangeCollider;
    private bool _HasPlayer = false;

    [Header("BossPattern_Special")]
    [SerializeField] private BossPattern _Appears;
    [SerializeField] private BossPattern _Groggy;
    [SerializeField] private BossPattern _ShoutingLong;

    [Header("BossPattern_Normal")]
    [SerializeField] private BossPattern[] _Patterns;
    private List<BossPattern> _CanActionPatterns;

    private void Awake()
    {
        _AnimControlKey = _Animator.GetParameter(0).nameHash;
        _CanActionPatterns = new List<BossPattern>(_Patterns.Length);

        _RangeCollider.OnTriggerAction += (Collider2D other, bool isEnter) =>
        {
            if (!other.CompareTag("Player")) return;

            if (_HasPlayer = isEnter)
            {
                for (int i = 0; i < _Patterns.Length; i++)
                    _Patterns[i].Notify_PlayerEnter();
            }
            else
            {
                for (int i = 0; i < _Patterns.Length; i++)
                    _Patterns[i].Notify_PlayerExit();
            }
        };
        // Special Pattern Init
        {
            _Appears.Init();
            _Groggy.Init();
            _ShoutingLong.Init();
        }
        for (int i = 0; i < _Patterns.Length; i++) 
        {
            _Patterns[i].Init();
        }
        _Appears.Action();
    }
    public void Awaken()
    {
        MainCamera.Instance.SetCameraScale(7.2f, 1f);

        _ShoutingLong.Action();
        StartCoroutine(PatternTimer());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) _ShoutingLong.Action();
        if (Input.GetKeyDown(KeyCode.X)) _Groggy.Action();
    }
    private IEnumerator PatternTimer()
    {
        while (_Animator.GetInteger(_AnimControlKey) != Idle)
            yield return null;

        while (gameObject.activeSelf)
        {
            for (float i = 0f; i < _PatternWait; i += Time.deltaTime * Time.timeScale)
                yield return null;

            while (_Animator.GetInteger(_AnimControlKey) != Idle)
                yield return null;

            CanActionPatternsUpdate();

            transform.localScale = (_HeartPoint.position.x < _PlayerTransform.localPosition.x)
                    ? LookRight : LookLeft;

            _CanActionPatterns[Random.Range(0, _CanActionPatterns.Count)].Action();

            while (_Animator.GetInteger(_AnimControlKey) != Idle)
                yield return null;
        }
    }
    private void CanActionPatternsUpdate()
    {
        _CanActionPatterns.Clear();

        for (int i = 0; i < _Patterns.Length; i++)
        {
            if (_Patterns[i].CanAction) {
                _CanActionPatterns.Add(_Patterns[i]);
            }
        }
    }
    private void AE_SetIdleState()
    {
        _Animator.SetInteger(_AnimControlKey, Idle);
    }
}

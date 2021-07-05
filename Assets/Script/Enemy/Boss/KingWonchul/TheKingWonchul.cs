using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheKingWonchul : MonoBehaviour
{
    private const int Idle = 0;

    [SerializeField] private Animator _Animator;
    private int _AnimControlKey;

    [SerializeField] private float _PatternWait;

    [Space()]
    [SerializeField] private BossPattern _Appears;
    [SerializeField] private BossPattern _SlashSlash;
    [SerializeField] private BossPattern _Slash;
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
        _SlashSlash.Init();
        _Appears.Action();
        _Slash.Init();
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

            switch (Random.Range(0, 4))
            {
                case 0:
                    _ShoutingShort.Action();
                    break;
                case 1:
                    _ShoutingLong.Action();
                    break;
                case 2:
                    _SlashSlash.Action();
                    break;
                case 3:
                    _Slash.Action();
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
}

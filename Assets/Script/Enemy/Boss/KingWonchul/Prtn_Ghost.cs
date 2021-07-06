using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prtn_Ghost : BossPattern
{
    private const int BrustCount = 8;
    private const float BurstHoldingTime = 2.3f;

    public override int AnimationCode => 10;
    public override bool CanAction => _HasPlayer;

    [Header("Ghost Property")]
    [SerializeField] private GhostProjectile _GhostProjectile;
    private Stack<GhostProjectile> _GhostPool;

    [SerializeField] private ParticleSystem _ParticleSystem;
    [SerializeField] private Transform _BrustPoint;

    private Transform _Player;

    public override void Init()
    {
        base.Init();
        _GhostPool = new Stack<GhostProjectile>();

        AddPoolObject();
        _Player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void AE_Ghost_Brust()
    {
        MainCamera.Instance.CameraShake(0.6f, 0.4f);
        StartCoroutine(BrustRoutine());

        _ParticleSystem.Play();
    }
    private IEnumerator BrustRoutine()
    {
        for (float i = 0f; i < 0.15f; i += Time.deltaTime * Time.timeScale)
        {
            if (CanRoutineBreak()) 
                yield break;
            
            yield return null;
        }
        GhostBrust();

        for (float i = 0f; i < BurstHoldingTime; i += Time.deltaTime * Time.timeScale)
        {
            if (CanRoutineBreak())
                yield break;

            yield return null;
        }
        AE_SetDefaultState();
    }
    private bool CanRoutineBreak()
    {
        return _Animator.GetInteger(_AnimatorHash) != AnimationCode;
    }
    private void GhostBrust()
    {
        Vector2 position = _BrustPoint.position;
        for (int i = 0; i < BrustCount; i++)
        {
            var ghost = _GhostPool.Pop();
            if (ghost == null)
            {
                AddPoolObject();
                ghost = _GhostPool.Pop();
            }
            ghost.transform.localPosition = position;
            ghost.Project(_Player);
        }
    }
    private void AddPoolObject()
    {
        Vector2 position = _BrustPoint.position;
        for (int i = 0; i < BrustCount; i++)
        {
            _GhostPool.Push(Instantiate(_GhostProjectile, position, Quaternion.identity));

            _GhostPool.Peek().ReleaseEvent += o => 
            {
                _GhostPool.Push(o);
            };
            _GhostPool.Peek().gameObject.SetActive(false);
        }
    }
}

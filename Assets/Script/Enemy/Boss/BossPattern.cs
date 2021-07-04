using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossPattern : MonoBehaviour
{
    [SerializeField] protected int _DefaultAnimationCode;
    [SerializeField] protected Animator _Animator;

    protected int _AnimatorHash;
    protected bool _HasPlayer;
    protected float _RestHealthPercent;

    public abstract int AnimationCode { get; }
    public virtual bool CanAction { get => true; }

    public virtual void Init()
    {
        _AnimatorHash = _Animator.GetParameter(0).nameHash;
    }
    public virtual void Action()
    {
        _Animator.SetInteger(_AnimatorHash, AnimationCode);
    }
    public virtual void Notify_HealthUpdate(float restPercent) 
    {
        _RestHealthPercent = restPercent; 
    }
    public virtual void Notify_PlayerEnter() { _HasPlayer =  true; }
    public virtual void Notify_PlayerExit()  { _HasPlayer = false; }
}

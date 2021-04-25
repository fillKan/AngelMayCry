using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase
{
    // 무기에 필요한 이넘, 구조체, 클래스 //
    public enum eWeapons
    {
        None,
        Glove,
        Sword,
        End
    }

    public enum eCommands
    {
        None,
        Front,
        Back,
        Up,
        Down,
        Left,
        Middle,
        Right
    }

    public enum eWeaponEvents
    {
        Cancelable,
        UnCancelable,
        QuickSwapable,
        AttackEnd
    }

    public enum eWeaponType
    {
        Melee,
        Ranged
    }

    [System.Serializable]
    public class WeaponProperty
    {
        public string Name { get; set; }
        public eWeaponType Type { get; set; }
    }

    protected WeaponProperty _Property = new WeaponProperty();
    protected bool _isCancelable = false;
    protected bool _isQuickSwapable = false;
    public bool isQuickSwapable { get => _isQuickSwapable; }
    protected Animator _Animator;
    protected Player _Player;

    public virtual void Attack(eCommands direction, eCommands key)
    {
        _isCancelable = false;
        _isQuickSwapable = false;
        _Player.State = CharacterBase.eState.Attack;
    }
    public virtual void HandleAnimationEvents(eWeaponEvents weaponEvent)
    {
        switch(weaponEvent)
        {
            case eWeaponEvents.Cancelable:
                _isCancelable = true;
                break;

            case eWeaponEvents.UnCancelable:
                _isCancelable = false;
                break;

            case eWeaponEvents.QuickSwapable:
                _Player.StartCoroutine(QuickSwapableTimerRoutine());
                break;

            case eWeaponEvents.AttackEnd:
                _Animator.Play("Player_Idle");
                _isCancelable = false;
                _Player.State = CharacterBase.eState.Idle;
                break;
        }
    }
    private IEnumerator QuickSwapableTimerRoutine()
    {
        _isQuickSwapable = true;
        yield return new WaitForSecondsRealtime(0.1f);
        _isQuickSwapable = false;
    }
	protected void PlayAnimation(string key, out bool isAttacked)
	{
		_Animator.Play(key);
		isAttacked = true;
	}
}

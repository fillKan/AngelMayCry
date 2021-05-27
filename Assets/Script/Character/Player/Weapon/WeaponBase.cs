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
	protected int _ComboCounter = 0;
    public bool isCancelable { get => _isCancelable; }
    protected Animator _Animator;
    protected Player _Player;

	public void OnSwap()
	{
		_isCancelable = false;
		_ComboCounter = 0;
	}
    public virtual void Attack(eCommands direction, eCommands key)
    {
        _isCancelable = false;
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

            case eWeaponEvents.AttackEnd:
				_Player.NextAnimation = "Player_Idle";
                _isCancelable = false;
				_Player.StartCoroutine(ComboCounterResetRoutine());
				_Player.State = CharacterBase.eState.Idle;
                break;
        }
    }
	private IEnumerator ComboCounterResetRoutine()
	{
		yield return new WaitForSecondsRealtime(0.1f);
		if (_Player.State != CharacterBase.eState.Attack)
			_ComboCounter = 0;
	}
	protected void PlayAnimation(string key, out bool isAttacked)
	{
		_Player.NextAnimation = key;
		isAttacked = true;
	}
}

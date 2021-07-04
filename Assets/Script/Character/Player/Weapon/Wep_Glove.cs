using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wep_Glove : WeaponBase
{

    public Wep_Glove(Player player, Animator animator)
    {
        _Player = player;
        _Animator = animator;
        _isCancelable = false;

        _Property.Name = "Glove";
        _Property.Type = eWeaponType.Melee;
    }

    public override void Attack(eCommands direction, eCommands key)
    {
        if (_Player.GetState() != CharacterBase.eState.Idle && _Player.GetState() != CharacterBase.eState.Move && !_isCancelable) // 서있거나 이동중이 아니고 캔슬이 불가능할 때
            return;
        if (_Player.GetComponent<Rigidbody2D>().velocity.y != 0) // 공중에 있을 때
            return;
        if (_isCancelable && direction == eCommands.None) // 캔슬이 가능하지만 입력한 커맨드가 평타일 때
            return;

        base.Attack(direction, key);

		bool isAttacked = false;
		switch (direction)
        {
            case eCommands.None:
                if (key == eCommands.Left)
                {
					PlayAnimation(_ComboCounter % 2 == 0 ? "Player_Glove_WeakAttack" : "Player_Glove_StrongAttack", out isAttacked);
					_ComboCounter++;
                }
                break;

            case eCommands.Front:
                if(key == eCommands.Right)
					PlayAnimation("Player_Glove_Smash", out isAttacked);
                else if(key == eCommands.Left)
					PlayAnimation("Player_Glove_Special", out isAttacked);
                break;

            case eCommands.Up:
                if (key == eCommands.Right)
					PlayAnimation("Player_Glove_Airborne", out isAttacked);
                break;
        }

        if(isAttacked == false) // 입력한 키에 맞는 공격이 없을 때
        {
            _Player.SetState(CharacterBase.eState.Idle);
        }
    }
    public override void HandleAnimationEvents(eWeaponEvents weaponEvent)
    {
        base.HandleAnimationEvents(weaponEvent);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wep_Glove : WeaponBase
{

    private int _AttackPhase = 0;

    public Wep_Glove(Player player, Animator animator)
    {
        _Player = player;
        _Animator = animator;
        _isCancelable = false;
        _isQuickSwapable = false;

        _Property.Name = "Glove";
        _Property.Type = eWeaponType.Melee;
    }

    public override void Attack(eCommands direction, eCommands key)
    {
        if (_Player.State != StateBase.eState.Idle && _Player.State != StateBase.eState.Move && !_isCancelable) // 서있거나 이동중이 아니고 캔슬이 불가능할 때
            return;
        if (_Player.GetComponent<Rigidbody2D>().velocity.y != 0) // 공중에 있을 때
            return;
        if (_isCancelable && direction == eCommands.None) // 캔슬이 가능하지만 입력한 커맨드가 평타일 때
            return;

        base.Attack(direction, key);

        switch(direction)
        {
            case eCommands.None:
                if (key == eCommands.Left)
                {
                    _Animator.Play(_AttackPhase % 2 == 0 ? "Player_Glove_WeakAttack" : "Player_Glove_StrongAttack");
                    _AttackPhase++;
                }
                break;

            case eCommands.Front:
                if(key == eCommands.Left)
                   _Animator.Play("Player_Glove_Smash");
                else if(key == eCommands.Right)
                    _Animator.Play("Player_Glove_Special");
                break;

            case eCommands.Up:
                if (key == eCommands.Left)
                    _Animator.Play("Player_Glove_Airborne");
                break;
        }
    }
    public override void HandleAnimationEvents(eWeaponEvents weaponEvent)
    {
        base.HandleAnimationEvents(weaponEvent);
    }
}

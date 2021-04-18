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
        if (_Player.State != StateBase.eState.Idle && _Player.State != StateBase.eState.Move && !_isCancelable) // ���ְų� �̵����� �ƴϰ� ĵ���� �Ұ����� ��
            return;
        if (_Player.GetComponent<Rigidbody2D>().velocity.y != 0) // ���߿� ���� ��
            return;
        if (_isCancelable && direction == eCommands.None) // ĵ���� ���������� �Է��� Ŀ�ǵ尡 ��Ÿ�� ��
            return;

        base.Attack(direction, key);

		bool isAttacked = false;
		switch (direction)
        {
            case eCommands.None:
                if (key == eCommands.Left)
                {
					PlayAnimation(_AttackPhase % 2 == 0 ? "Player_Glove_WeakAttack" : "Player_Glove_StrongAttack", out isAttacked);
                    _AttackPhase++;
                }
                break;

            case eCommands.Front:
                if(key == eCommands.Left)
					PlayAnimation("Player_Glove_Smash", out isAttacked);
                else if(key == eCommands.Right)
					PlayAnimation("Player_Glove_Special", out isAttacked);
                break;

            case eCommands.Up:
                if (key == eCommands.Left)
					PlayAnimation("Player_Glove_Airborne", out isAttacked);
                break;
        }

        if(isAttacked == false) // �Է��� Ű�� �´� ������ ���� ��
        {
            _Player.State = StateBase.eState.Idle;
        }
    }
    public override void HandleAnimationEvents(eWeaponEvents weaponEvent)
    {
        base.HandleAnimationEvents(weaponEvent);
    }
}

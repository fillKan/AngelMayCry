using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wep_Sword : WeaponBase
{
	private int _AttackPhase = 0;

	public Wep_Sword(Player player, Animator animator)
	{
		_Player = player;
		_Animator = animator;
		_isCancelable = false;
		_isQuickSwapable = false;

		_Property.Name = "Sword";
		_Property.Type = eWeaponType.Melee;
	}

	public override void Attack(eCommands direction, eCommands key)
	{
		if (_Player.State != CharacterBase.eState.Idle && _Player.State != CharacterBase.eState.Move && !_isCancelable) // ���ְų� �̵����� �ƴϰ� ĵ���� �Ұ����� ��
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
					switch(_AttackPhase)
					{
						case 0: PlayAnimation("Player_Sword_Swing1", out isAttacked); break;
						case 1: PlayAnimation("Player_Sword_Swing2", out isAttacked); break;
						case 2: PlayAnimation("Player_Sword_Swing3", out isAttacked); break;
					}
					_AttackPhase = (_AttackPhase + 1) % 3;
				}
				break;

			case eCommands.Front:
				if (key == eCommands.Right)
					PlayAnimation("Player_Sword_ShieldSlam", out isAttacked);
				else if (key == eCommands.Left)
				{
					switch (_AttackPhase)
					{
						case 0: PlayAnimation("Player_Sword_Swing1", out isAttacked); break;
						case 1: PlayAnimation("Player_Sword_Swing2", out isAttacked); break;
						case 2: PlayAnimation("Player_Sword_Swing3", out isAttacked); break;
					}
					_AttackPhase = (_AttackPhase + 1) % 3;
				}
				break;

			case eCommands.Down:
				if (key == eCommands.Left)
					PlayAnimation("Player_Sword_Parrying_Ready", out isAttacked);
				break;
		}

		if (isAttacked == false) // �Է��� Ű�� �´� ������ ���� ��
		{
			_Player.State = CharacterBase.eState.Idle;
		}
	}
	public override void HandleAnimationEvents(eWeaponEvents weaponEvent)
	{
		base.HandleAnimationEvents(weaponEvent);
	}
}

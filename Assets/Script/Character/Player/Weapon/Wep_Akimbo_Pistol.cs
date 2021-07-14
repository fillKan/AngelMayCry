using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wep_Akimbo_Pistol : WeaponBase
{
	public Wep_Akimbo_Pistol(Player player, Animator animator)
	{
		_Player = player;
		_Animator = animator;
		_isCancelable = false;

		_Property.Name = "Akimbo_Pistol";
		_Property.Type = eWeaponType.Ranged;
	}

	private IEnumerator _SpinMoveRoutine = null;

	public override void Attack(eCommands direction, eCommands key)
	{
		if (_Player.GetState() != CharacterBase.eState.Idle && _Player.GetState() != CharacterBase.eState.Move && !_isCancelable) // ���ְų� �̵����� �ƴϰ� ĵ���� �Ұ����� ��
			return;
		if (_Player.GetComponent<Rigidbody2D>().velocity.y != 0) // ���߿� ���� ��
			return;
		if (_isCancelable && ((direction == eCommands.None) || (direction == eCommands.Front && key == eCommands.Left))) // ĵ���� ���������� �Է��� Ŀ�ǵ尡 ��Ÿ�� ��
			return;

		bool isAttacked = false;
		switch (direction)
		{
			case eCommands.None:
				if (key == eCommands.Left)
				{
					PlayAnimation("Player_Akimbo_Pistol_WeakAttack", out isAttacked);
				}
				break;

			case eCommands.Front:
				if (key == eCommands.Right)
				{
					PlayAnimation("Player_Akimbo_Pistol_Spin", out isAttacked);
					_SpinMoveRoutine = MoveWhileSpinRoutine(_Player);
					_Player.StartCoroutine(_SpinMoveRoutine);
				}
				else if (key == eCommands.Left)
				{
					PlayAnimation("Player_Akimbo_Pistol_WeakAttack", out isAttacked);
				}
				break;

			case eCommands.Up:
				if (key == eCommands.Right)
					PlayAnimation("Player_Akimbo_Pistol_Vault", out isAttacked);
				break;

			case eCommands.Down:
				if (key == eCommands.Right)
					PlayAnimation("Player_Akimbo_Pistol_ChargeShot", out isAttacked);
				break;
		}

		if (isAttacked == false && _isCancelable == false) // �Է��� Ű�� �´� ������ ���� ��
		{
			_Player.SetState(CharacterBase.eState.Idle);
			return;
		}
		base.Attack(direction, key);
	}
	public override void HandleAnimationEvents(eWeaponEvents weaponEvent)
	{
		base.HandleAnimationEvents(weaponEvent);
		if (weaponEvent == eWeaponEvents.Akimbo_Pistol_SpinMoveEnd)
		{
			_Player.StopCoroutine(_SpinMoveRoutine);
		}
	}
	private IEnumerator MoveWhileSpinRoutine(Player player)
	{
		yield return null;
		Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
		while (player.GetState() == CharacterBase.eState.Attack)
		{
			if (Input.GetKey(KeyCode.A))
			{
				rigid.AddForce(new Vector2(-1500 * Time.deltaTime * Time.timeScale, 0));
				rigid.velocity = new Vector2(Mathf.Clamp(rigid.velocity.x, -5, 5), rigid.velocity.y);
				Vector3 Scale = player.transform.localScale;
				Scale.x = Mathf.Abs(Scale.x) * -1;
				player.transform.localScale = Scale;
			}
			else if (Input.GetKey(KeyCode.D))
			{
				rigid.AddForce(new Vector2(1500 * Time.deltaTime * Time.timeScale, 0));
				rigid.velocity = new Vector2(Mathf.Clamp(rigid.velocity.x, -5, 5), rigid.velocity.y);
				Vector3 Scale = player.transform.localScale;
				Scale.x = Mathf.Abs(Scale.x);
				player.transform.localScale = Scale;
			}
			yield return null;
		}
	}
}

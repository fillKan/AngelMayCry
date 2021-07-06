using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
	public enum eHitEvents
	{
		None
	}

	[Header("성능")]
	[SerializeField]
	private float _Damage = 0;
	[SerializeField]
	private Vector2 _Knockback = new Vector2(0,0);
	[SerializeField]
	[Tooltip("맞은 적이 행동불능이 되는 시간")]
	private float _StunTime = 0;
	[SerializeField]
	[Tooltip("기타 피격시 상호작용이 필요한 경우 사용하는 열거자")]
	private eHitEvents _Event = eHitEvents.None;

	[Header("이펙트")]
	[SerializeField]
	private float _CameraShakeForce = 0;
	[SerializeField]
	[Tooltip("-1로 두면 HitStop을 따라감")]
	private float _CameraShakeTime = -1;
	[SerializeField]
	[Tooltip("피격시 게임이 잠시 멈추는 이펙트")]
	private float _HitStop = 0;
	[SerializeField]
	private GameObject _HitParticle = null;
	[SerializeField]
	private string _HitSound = "PlaceHolder";

	List<GameObject> _CollidedObjects = new List<GameObject>();

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (gameObject.tag == "PlayerHitBox" && collision.tag == "PlayerHurtBox")
			return;
		if (gameObject.tag == "EnemyHitBox" && collision.tag == "EnemyHurtBox")
			return;

		if (collision.transform.root.TryGetComponent(out CharacterBase Other))
        {
			if (Other.GetState() == CharacterBase.eState.Down || 
				Other.GetState() == CharacterBase.eState.Dead || 
				Other.GetState() == CharacterBase.eState.Wake) return;

			for (int i = 0; i < _CollidedObjects.Count; i++) // 한 오브젝트가 두 번 충돌하는걸 방지
			{
				if (Other.gameObject == _CollidedObjects[i])
					return;
			}
			_CollidedObjects.Add(Other.gameObject);

			MainCamera.Instance.CameraShake(_CameraShakeTime == -1 ? _HitStop * 2f : _CameraShakeTime, _CameraShakeForce);
			TimeManager.Instance.HitStop(_HitStop);
			_Knockback.x *= Mathf.Sign(transform.root.localScale.x);

			Other.DealDamage(_Damage, _StunTime, _Knockback, transform.root.gameObject);
		}
	}

	private void OnEnable()
	{
		_CollidedObjects.Clear();
	}
}

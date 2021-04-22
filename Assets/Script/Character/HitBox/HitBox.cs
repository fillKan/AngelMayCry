using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
	public enum eHitEvents
	{
		None
	}

	[Header("����")]
	[SerializeField]
	private float _Damage = 0;
	[SerializeField]
	private Vector2 _Knockback = new Vector2(0,0);
	[SerializeField]
	[Tooltip("���� ���� �ൿ�Ҵ��� �Ǵ� �ð�")]
	private float _StunTime = 0;
	[SerializeField]
	[Tooltip("��Ÿ �ǰݽ� ��ȣ�ۿ��� �ʿ��� ��� ����ϴ� ������")]
	private eHitEvents _Event = eHitEvents.None;

	[Header("����Ʈ")]
	[SerializeField]
	private float _CameraShakeForce = 0;
	[SerializeField]
	[Tooltip("-1�� �θ� HitStop�� ����")]
	private float _CameraShakeTime = -1;
	[SerializeField]
	[Tooltip("�ǰݽ� ������ ��� ���ߴ� ����Ʈ")]
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
		GameObject Other = collision.transform.root.gameObject; 
		for(int i = 0; i < _CollidedObjects.Count; i++) // �� ������Ʈ�� �� �� �浹�ϴ°� ����
		{
			if (Other == _CollidedObjects[i])
				return;
		}
		_CollidedObjects.Add(Other);
		
		MainCamera.Instance.CameraShake(_CameraShakeTime == -1 ? _HitStop : _CameraShakeTime, _CameraShakeForce);
		TimeManager.Instance.HitStop(_HitStop);
		Other.GetComponent<CharacterBase>().DealDamage(_Damage, _StunTime, _Knockback, gameObject);
	}

	private void OnDisable()
	{
		_CollidedObjects.Clear();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponsUI : MonoBehaviour
{
	private GameObject[] _Weapons = null;
	private int _CurWeapon = 0;
	[EnumNamedArray(typeof(WeaponBase.eWeapons)), SerializeField]
	private Sprite[] _WeaponIcons = new Sprite[(int)WeaponBase.eWeapons.End];

	public void SwapWeapon(int weapon)
	{
		StartCoroutine(WeaponIconScalingRoutine(_CurWeapon, new Vector2(1, 1)));
		_CurWeapon = weapon;
		StartCoroutine(WeaponIconScalingRoutine(_CurWeapon, new Vector2(1.3f, 1.3f)));
	}

	private IEnumerator WeaponIconScalingRoutine(int index, Vector2 to)
	{
		float curSize = 1;
		Vector2 startSize = _Weapons[index].transform.localScale;
		while(curSize > 0.05f)
		{
			curSize *= Mathf.Pow(0.8f, 60f * Time.deltaTime);
			_Weapons[index].transform.localScale = Vector2.Lerp(to, startSize, curSize);
			yield return null;
		}
		_Weapons[index].transform.localScale = to;
	}

	public void UpdateCurrentWeapons(WeaponBase.eWeapons[] currentWeapons)
	{
		if(_Weapons == null)
		{
			_Weapons = new GameObject[5];
			for (int i = 0; i < 5; i++)
			{
				_Weapons[i] = transform.GetChild(i).gameObject;
			}
		}

		for(int i = 0;i < 5; i++)
		{
			_Weapons[i].GetComponent<Image>().sprite = _WeaponIcons[(int)currentWeapons[i]];
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_WeaponSwap : MonoBehaviour
{
	[EnumNamedArray(typeof(WeaponBase.eWeapons)), SerializeField]
	private Color[] _Colors = new Color[(int)WeaponBase.eWeapons.End];
	private List<ParticleSystem> _Particles = new List<ParticleSystem>();

	private void Start()
	{
		_Particles.Add(GetComponent<ParticleSystem>());
		for(int i = 0; i < transform.childCount; i++)
		{
			_Particles.Add(transform.GetChild(i).GetComponent<ParticleSystem>());
		}
	}

	public void Play(WeaponBase.eWeapons weapon)
	{
		for(int i = 0; i < _Particles.Count; i++)
		{
			ParticleSystem.MainModule temp = _Particles[i].main;
			temp.startColor = _Colors[(int)weapon];
		}
		_Particles[0].Play();
	}
}

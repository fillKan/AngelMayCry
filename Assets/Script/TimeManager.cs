using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
	float _RestHitStopTime = 100000;
	
    public void HitStop(float time, float speed = 0.1f)
	{
		Time.timeScale = speed;
		_RestHitStopTime = time;
	}

	private void Update()
	{
		_RestHitStopTime -= Time.unscaledDeltaTime;
		if(_RestHitStopTime <= 0)
		{
			Time.timeScale = 1;
			_RestHitStopTime = 100000;
		}
	}
}

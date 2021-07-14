using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedScaleUI : MonoBehaviour
{
	private Vector2 _OriginalScale;
	private void Start()
	{
		_OriginalScale = transform.localScale;
	}
	void FixedUpdate()
    {
		transform.localScale = new Vector2(_OriginalScale.x / transform.root.localScale.x, _OriginalScale.y / transform.root.localScale.y);
    }
}

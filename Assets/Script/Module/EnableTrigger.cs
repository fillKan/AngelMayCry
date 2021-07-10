using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _EnableTarget;
    [SerializeField] private string _TriggerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_TriggerTag))
        {
            _EnableTarget.SetActive(true);
        }
    }
}

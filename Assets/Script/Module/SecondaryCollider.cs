using System;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryCollider : MonoBehaviour
{
    public delegate void TriggerAction(Collider2D other, bool isEnter);
    public TriggerAction OnTriggerAction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerAction?.Invoke(collision, true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        OnTriggerAction?.Invoke(collision, false);
    }
}

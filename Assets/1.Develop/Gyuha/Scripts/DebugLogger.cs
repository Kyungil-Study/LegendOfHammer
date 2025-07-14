using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogger : MonoBehaviour
{
    private void Start()
    {
        BattleEventManager.Instance.Callbacks.OnTakeDamage += DamageLog;
    }

    private void DamageLog(TakeDamageEventArgs eventArgs)
    {
        Debug.Log($"{eventArgs.Attacker} -> {eventArgs.Target} : {eventArgs.Damage} damage");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BattleEventManager : MonoSingleton<BattleEventManager>
{
    public class EventCallbacks
    {
        public Action<StartBattleEventArgs> OnStartBattle;
        public Action<EndBattleEventArgs> OnEndBattle;
        public Action<TakeDamageEventArgs> OnTakeDamage;
    }
    public EventCallbacks Callbacks { get; private set; } = new EventCallbacks();

    public void CallEvent(BattleEventArgs eventArgs)
    {
        if(eventArgs is StartBattleEventArgs startEvent)
        {
            Callbacks.OnStartBattle?.Invoke(startEvent);
        }
        else if(eventArgs is EndBattleEventArgs endEvent)
        {
            Callbacks.OnEndBattle?.Invoke(endEvent);
        }
        else if(eventArgs is TakeDamageEventArgs damageEvent)
        {
            Callbacks.OnTakeDamage?.Invoke(damageEvent);
        }
    }
}

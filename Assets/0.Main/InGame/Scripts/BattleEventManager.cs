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
        public Action<AliveMonsterEventArgs> OnAliveMonster;
        public Action<DeathEventArgs> OnDeath;
        public Action<NextPageEventArgs> OnNextPage;
        
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
            damageEvent.Target.TakeDamage(damageEvent);
            Callbacks.OnTakeDamage?.Invoke(damageEvent);
        }
        else if(eventArgs is AliveMonsterEventArgs aliveMonsterEvent)
        {
            Callbacks.OnAliveMonster?.Invoke(aliveMonsterEvent);
        }
        else if(eventArgs is DeathEventArgs deathEvent)
        {
            Callbacks.OnDeath?.Invoke(deathEvent);
        }
        else if (eventArgs is NextPageEventArgs nextPageEvent)
        {
            Callbacks.OnNextPage?.Invoke(nextPageEvent);
        }
        else
        {
            Debug.LogWarning($"[BattleEventManager] Unhandled event type: {eventArgs.GetType().Name}");
        }
    }
}

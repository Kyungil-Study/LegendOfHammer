using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ArcherArrow : HeroProjectile
{
    public bool b_CanPenetrate = false;
    public int penetrateLimit = 0;
    public Monster targetMonster;
    
    // TODO: Find strongest enemy
    // 1보스
    // 2엘리트
    // 3최대체력
    protected override void Hit(Monster target)
    {
        penetrateLimit--;
        
        TakeDamageEventArgs eventArgs = new TakeDamageEventArgs(
            Squad.Instance,
            target,
            damage
        );
        BattleEventManager.Instance.CallEvent(eventArgs);
        
        if (penetrateLimit <= 0)
        {
            Destroy(gameObject);
        }
    }
}

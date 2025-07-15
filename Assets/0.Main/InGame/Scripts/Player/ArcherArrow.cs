using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ArcherArrow : HeroProjectile
{
    public bool b_CanPenetrate = false;
    public int penetrateLimit = 0;
    public Monster targetMonster;
    
    protected override GameObject FindTarget()
    {
       var monsters =  BattleManager.Instance.GetAllMonsters();
       if (monsters.Any() == false)
       {
           Debug.Log("No monsters found to target.");
           return null;
       }
       monsters = monsters.OrderByDescending(monster =>
       {
           var id = monster.EnemyID;
           var data = EnemyDataManager.Instance.Records[id];
           return data.Enemy_Rank;
       }).ThenByDescending(monster => monster.MaxHP);
       return monsters.First().gameObject;
    }

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

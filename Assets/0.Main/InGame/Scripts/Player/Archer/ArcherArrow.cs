using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class ArcherArrow : HeroProjectile
{
    public int pierceLimit = 0;
    public Monster targetMonster;
    public float targetAdditionalDamageFactor = 1;
    
    public Func<bool,int> DamageCalculationFunc { get; set; }
    public override int Damage => 
        DamageCalculationFunc == null ? 
            Owner.CalculateDamage(IsCritical) : 
            DamageCalculationFunc.Invoke(IsCritical);

    /// <summary>
    /// 등급, 최대 체력, 거리를 기준으로 정렬 후 가장 먼저 나오는 몬스터를 타겟으로 설정합니다.
    /// <br />* 등급이 높은 몬스터가 우선
    /// <br />* 등급이 같다면 최대 체력이 높은 몬스터가 우선
    /// <br />* 최대 체력도 같다면 플레이어와의 거리가 가까운 몬스터가 우선
    /// <br />// 만약 타겟이 없다면 null을 반환합니다.
    /// </summary>
    /// <returns></returns>
    protected override GameObject FindTarget()
    {
       var monsters =  BattleManager.Instance.GetAllMonsters();
       if (monsters.Any() == false)
       {
           //Debug.Log("No monsters found to target.");
           return null;
       }

       monsters = monsters.OrderByDescending(monster =>
           {
               var id = monster.EnemyID;
               var data = EnemyDataManager.Instance.Records[id];
               return data.Enemy_Rank;
           })
           .ThenByDescending(monster => monster.Stat.CurrentHP)
           .ThenBy(monster => Vector3.Distance(monster.transform.position, transform.position));
       targetMonster = monsters.First();
       return targetMonster.gameObject;
    }

    protected override void Hit(Monster target)
    {
        OnHit?.Invoke();
        pierceLimit--;

        bool isSuccesHitTarget = targetMonster != null ? targetMonster.Equals(target) : false;
        targetMonster = target;

        float tagetFactor = isSuccesHitTarget ? 1 : targetAdditionalDamageFactor;
        int FinalDamage = (int)(Damage * tagetFactor);
        //Debug.Log($" [ArcherArrow] Hit {target.EnemyID} with damage: {FinalDamage} (pierce limit: {pierceLimit})");
        TakeDamageEventArgs eventArgs = new TakeDamageEventArgs(
            Squad.Instance,
            target,
            FinalDamage 
        );
        BattleEventManager.Instance.CallEvent(eventArgs);
        
        if (pierceLimit <= 0)
        {
            Destroy(gameObject);
        }
    }

}

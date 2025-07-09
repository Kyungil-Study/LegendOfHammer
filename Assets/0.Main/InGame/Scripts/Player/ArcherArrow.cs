using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.Serialization;

public class ArcherArrow : HeroProjectile
{
    // TODO: Find strongest enemy
    // 1보스
    // 2엘리트
    // 3최대체력
    protected override void Hit(IBattleCharacter target)
    {
        TakeDamageEventArgs eventArgs = new TakeDamageEventArgs(
            Squad.Instance,
            target,
            damage
        );
        BattleEventManager.Instance.CallEvent(eventArgs);
        Destroy(gameObject);
    }
}

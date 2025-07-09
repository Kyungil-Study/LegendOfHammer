using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPatternSlot : MonoBehaviour
{
    [SerializeField] private EnemyRankType rankType;
    public EnemyRankType RankType => rankType;
    [SerializeField] private EnemyAttackType attackType;
    public EnemyAttackType AttackType => attackType;
}

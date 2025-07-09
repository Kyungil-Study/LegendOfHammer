using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnPatternSlot : MonoBehaviour
{
    [FormerlySerializedAs("rankType")] [SerializeField] private EnemySpawnRankType spawnRankType;
    public EnemySpawnRankType SpawnRankType => spawnRankType;
    [FormerlySerializedAs("attackType")] [SerializeField] private EnemySpawnAttackType spawnAttackType;
    public EnemySpawnAttackType SpawnAttackType => spawnAttackType;
}

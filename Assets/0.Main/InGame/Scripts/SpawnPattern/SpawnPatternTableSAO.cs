using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WaveTable", menuName = "ScriptableObjects/WaveTable", order = 1)]
public class SpawnPatternTableSAO : ScriptableObject
{
    [FormerlySerializedAs("waves")] [SerializeField] private SpawnPattern[] spawnPatterns;

    public void Resolve()
    {
        foreach (var spawnPattern in spawnPatterns)
        {
            if (spawnPattern == null)
            {
                Debug.LogError("[WaveTable] Wave가 null입니다. Wave를 확인해주세요.");
                continue;
            }
            spawnPattern.ResolveWaveSlots();
            
        }
    }

    public SpawnPattern GetSpawnPattern(EnemySpawnPatternType key)
    {
        foreach (SpawnPattern spawnPattern in spawnPatterns)
        {
            if (spawnPattern.PatternType == key)
            {
                return spawnPattern;
            }
        }
        
        Debug.LogError($"[WaveTable] WaveType {key}를 찾을 수 없습니다.");
        return null;
    }
    
}

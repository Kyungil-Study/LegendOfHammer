using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnPattern : MonoBehaviour
{
    [SerializeField] private bool onlyMid = false;
    public bool OnlyMid => onlyMid;
    [FormerlySerializedAs("waveType")] [SerializeField] private EnemySpawnPatternType patternType;
    public EnemySpawnPatternType PatternType => patternType;
    [FormerlySerializedAs("waveSlots")] [SerializeField] SpawnPatternSlot[] patternSlots;
    public SpawnPatternSlot[] PatternSlots => patternSlots;
    
    public void ResolveWaveSlots()
    {
        patternSlots = GetComponentsInChildren<SpawnPatternSlot>();
        if (patternSlots == null || patternSlots.Length == 0)
        {
            Debug.LogError($"[Wave] {gameObject.name}의 WaveSlots가 비어있습니다. WaveSlot을 추가해주세요.");
            return;
        }

        foreach (var slot in patternSlots)
        {
            if (slot == null)
            {
                Debug.LogError($"[Wave] {gameObject.name}의 WaveSlots에 null이 있습니다. WaveSlot을 확인해주세요.");
                continue;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnPattern : MonoBehaviour
{
    [LabelText("웨이브 종류 ID")] public int augmentTypeID = 0;
    [LabelText("웨이브 등급")] public WaveRankType WaveRank;
    [LabelText("웨이브 등장 최소 스테이지")] public int AppearMinStage = 999;
    [LabelText("웨이브 등장 최종 스테이지")] public int AppearMaxStage = 999;
    
    [SerializeField] private bool onlyMid = false;
    public bool OnlyMid => onlyMid;
    [LabelText("삭제될 옵션"), SerializeField] private EnemySpawnPatternType patternType;
    public EnemySpawnPatternType PatternType => patternType;
    [SerializeField] SpawnPatternSlot[] patternSlots;
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

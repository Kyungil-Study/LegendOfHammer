#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(SpawnPatternTableSAO))]
public class SpawnPatternTableSAOEditor : OdinEditor
{
    private int stageFilter = 1;
    private WaveRankType rankFilter = WaveRankType.Normal;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== 필터링된 스폰 패턴 미리보기 ===", EditorStyles.boldLabel);

        stageFilter = EditorGUILayout.IntField("스테이지", stageFilter);
        rankFilter = (WaveRankType)EditorGUILayout.EnumPopup("웨이브 등급", rankFilter);

        var table = (SpawnPatternTableSAO)target;
        var results = table.FilteredSpawnPatterns(stageFilter, rankFilter);

        if (results.Count == 0)
        {
            EditorGUILayout.HelpBox("해당 조건에 맞는 패턴이 없습니다.", MessageType.Info);
        }

        foreach (var pattern in results)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"패턴 ID: {pattern.augmentTypeID}, 타입: {pattern.PatternType}, 슬롯 개수: {pattern.PatternSlots?.Length ?? 0}");
            EditorGUI.indentLevel++;
            if (pattern.PatternSlots != null)
            {
                foreach (var slot in pattern.PatternSlots)
                {
                    if (slot == null) continue;
                    EditorGUILayout.LabelField($"- Rank: {slot.SpawnRankType}, Attack: {slot.SpawnAttackType}");
                }
            }
            EditorGUI.indentLevel--;
        }
    }
}
#endif
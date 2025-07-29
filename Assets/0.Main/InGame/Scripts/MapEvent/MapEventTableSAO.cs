using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "MapEventTable", menuName = "MapEvent/MapEventTable", order = 1)]
public class MapEventTableSAO : ScriptableObject
{
    [LabelText("맵 이벤트 패턴 종류"), SerializeField] List<MapEventPatternSAO> mapEventPatterns;
    
    public IReadOnlyList<MapEventPatternSAO> FiltertedMapEventPatterns(int stage)
    {
        return mapEventPatterns
            .Where(pattern => pattern.FirstAppearStage <= stage && pattern.MaxAppearStage >= stage)
            .ToList();
    }
}

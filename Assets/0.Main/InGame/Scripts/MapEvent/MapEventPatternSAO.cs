using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class MapEventPatternSAO : ScriptableObject
{
    [Space(10), Header("맵 이벤트 설정")]
    [LabelText("맵 이벤트 ID")] public int MapEventID;
    [LabelText("맵 이벤트명")] public string MapEventName;
    [LabelText("공격력")] public int MapEventDamage;
    [LabelText("최초 등장 스테이지")] public int FirstAppearStage;
    [LabelText("최대 등장 스테이지")] public int MaxAppearStage;
    
    public abstract void ExecuteEvent();
}


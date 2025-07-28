using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireballPointType
{
    Upper,
    Left,
}

[Serializable]
public class FireballSpawnJob
{
    [SerializeField] float attackDelay = 0f;
    [SerializeField] FireballPointType pointType = FireballPointType.Upper;
}


[CreateAssetMenu(fileName = "FireballPattern", menuName = "MapEvent/FireballPattern", order = 1)]
public class FireballPattern : ScriptableObject
{
    public int patternId;
    public string patternName;
    public int appearanceMinStage = 0;
    public int appearanceMaxStage = 0;
    [SerializeField] public List<FireballSpawnJob> fireballSpawnJobs = new List<FireballSpawnJob>();
    
    
}

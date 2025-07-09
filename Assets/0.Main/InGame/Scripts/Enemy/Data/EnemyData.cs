using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CsvHelper.Configuration.Attributes;

public enum EnemyRank
{ 
    Normal,
    Elite,
    Boss
}

public enum MovementPattern
{
    Straight,
    Zigzag,
    Chase,
    Flying
}

public enum AttackPattern
{
    Normal,
    Suicide,
    Shield,
    Sniper,
    Spread,
    Radial,
    Flying
}

public class EnemyData
{
    public int Enemy_ID { get; set; }
    public EnemyRank Enemy_Rank { get; set; }
    public bool Is_Ranged { get; set; }
    public string Enemy_Unit_Name { get; set; }
    public MovementPattern Movement_Pattern { get; set; }
    public AttackPattern Atk_Pattern { get; set; }
    public int? HP { get; set; } // 체력 (int) : 빈 값 때문에 0 처리해야
    public int Atk_Power { get; set; }
    public float Move_Speed { get; set; }
    public int Chasing_Increase { get; set; }
    public int First_Appear_Stage { get; set; }
}
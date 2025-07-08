using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CsvHelper.Configuration.Attributes;

[System.Serializable]
public enum EnemyRank      { Normal, Elite, Boss }

[System.Serializable]
public enum EnemyMovement
{
    Straight,
    Zigzag, 
    Chase,
    Runner,
    Shield,
    Sniper,
    Spread,
    Radial,
    Flying,
}

[System.Serializable]
public class EnemyRecord
{
    public int             EnemyID            { get; set; }
    public EnemyRank       Rank               { get; set; }
    public bool            Is_Ranged          { get; set; }
    public string          UnitName           { get; set; }
    public EnemyMovement   Movement_Pattern   { get; set; }
    public string          Atk_Pattern        { get; set; }
    public int             HP                 { get; set; }
    public int             Atk_Power          { get; set; }
    public float           Move_Speed         { get; set; }
    public int             Chasing_Increase   { get; set; }
    public int             First_Appear_Stage { get; set; }
}

/*public class EnemyRecord
{
    [Index(0)]  public int             EnemyID            { get; set; }
    [Index(1)]  public EnemyRank       Rank               { get; set; }
    [Index(2)]  public bool            Is_Ranged          { get; set; }
    [Index(3)]  public string          UnitName           { get; set; }
    [Index(4)]  public EnemyMovement   Movement_Pattern   { get; set; }
    [Index(5)]  public string          Atk_Pattern        { get; set; }
    [Index(6)]  public int             HP                 { get; set; }
    [Index(7)]  public int             Atk_Power          { get; set; }
    [Index(8)]  public float           Move_Speed         { get; set; }
    [Index(9)]  public int             Chasing_Increase   { get; set; }
    [Index(10)] public int             First_Appear_Stage { get; set; }
}*/
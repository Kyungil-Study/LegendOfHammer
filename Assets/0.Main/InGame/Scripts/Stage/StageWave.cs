using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaveRankType
{
    Normal,
    Elite,
    Boss
}

public class StageWave
{
    public float PlayTime { get; set; }
    public bool WaveEmerge { get; set; }
    public WaveRankType WaveRank { get; set; }
    public bool ObstacleEmerge { get; set; }
    public bool MapEventEmerge { get; set; }

    // Constructor to initialize the properties
    public StageWave(StageWaveTSV tsvData)
    {
        PlayTime = tsvData.Play_Time;
        WaveEmerge = tsvData.Wave_Emerge;
        WaveRank = tsvData.Wave_Rank;
        ObstacleEmerge = tsvData.Obstacle_Emerge;
        MapEventEmerge = tsvData.Map_Event_Emerge;
    }
    
    public override string ToString()
    {
        return $"PlayTime: {PlayTime}, WaveEmerge: {WaveEmerge}, WaveRank: {WaveRank}, ObstacleEmerge: {ObstacleEmerge}, MapEventEmerge: {MapEventEmerge}";
    }
}

public class StageWaveTSV
{
    public float Play_Time { get; set; }
    public bool Wave_Emerge { get; set; }
    public WaveRankType Wave_Rank { get; set; }
    public bool Obstacle_Emerge { get; set; }
    public bool Map_Event_Emerge { get; set; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaveRankType
{
    Normal,
    Elite,
    Boss,
    No_Emerge
}

public class StageWave
{
    public float PlayTime { get; set; }
    public bool WaveEmerge { get; set; }
    public WaveRankType WaveRank { get; set; }
    public bool ObstacleEmerge { get; set; }
    public bool MapEventEmerge { get; set; }

    public override string ToString()
    {
        return $"PlayTime: {PlayTime}, WaveEmerge: {WaveEmerge}, WaveRank: {WaveRank}, ObstacleEmerge: {ObstacleEmerge}, MapEventEmerge: {MapEventEmerge}";
    }
}

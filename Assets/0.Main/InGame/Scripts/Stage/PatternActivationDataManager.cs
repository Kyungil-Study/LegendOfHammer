using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Threading.Tasks;
using UnityEngine;

public class PatternActivation
{
    public int ID { get; set; }
    public EnemySpawnPatternType PatternName { get; set; }
    public WaveRankType Rank { get; set; }
    public bool OnlyMid { get; set; }
    public int NormalMelee { get; set; }
    public int NormalRange { get; set; }
    public int EliteMelee { get; set; }
    public int EliteRange { get; set; }
    public int BossMelee { get; set; }
    public int BossRange { get; set; }
    public int AppearStage { get; set; }
    public int DisappearStage { get; set; }
}

public class PatternActivationDataManager : MonoSingleton<PatternActivationDataManager>
{
    [SerializeField] private string resourcePath = "PatternActivationData";
    List<PatternActivation> records = new();
    public IReadOnlyList<PatternActivation> Records
    {
        get
        {
            if (records == null)
            {
                throw new InvalidOperationException("Records not initialized. Call Load() first.");
            }
            return records;
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        records = TSVLoader.LoadTable<PatternActivation>(resourcePath);
    }
}



using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class StageDataManager : MonoSingleton<StageDataManager>
{
    [SerializeField] public List<StageTable> StageTables = new List<StageTable>();
    [Serializable]
    public class StageTable
    {
        [SerializeField] public string TablePath;
        public List<StageWave> Waves { get; set; }
        
        [SerializeField] public int UseMinStageIndex = 0;
        [SerializeField] public int UseMaxStageIndex = 0;
        
        public StageTable()
        {
            Waves = new List<StageWave>();
        }

        public void AddWave(StageWave wave)
        {
            if (wave == null)
            {
                throw new ArgumentNullException(nameof(wave), "Wave cannot be null.");
            }
            Waves.Add(wave);
        }

        public override string ToString()
        {
            return $"StageTable with {Waves.Count} waves.";
        }
        
    }
    
    private StageTable currentStageTable;
    
    
    public IReadOnlyList<StageWave> Waves(int stageIndex)
    {
        foreach (var table in StageTables)
        {
            if (stageIndex >= table.UseMinStageIndex && stageIndex <= table.UseMaxStageIndex)
            {
                currentStageTable = table;
                return table.Waves;
            }
        }
        
        throw new ArgumentOutOfRangeException(nameof(stageIndex), "Stage index is out of range for all stage tables.");
    }

    protected override void Initialize()
    {
        base.Initialize();

        foreach (var table in StageTables)
        {
            if (string.IsNullOrEmpty(table.TablePath))
            {
                throw new ArgumentNullException(nameof(table), "Resource path cannot be null or empty.");
            }
            table.Waves = TSVLoader.LoadTable<StageWave>(table.TablePath);
            
        }
        
    }

}

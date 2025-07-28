using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Serialization;

public class EnemyDataManager : MonoSingleton<EnemyDataManager>
{
    [SerializeField] private string enemyDataTablePath = "EnemyData";
    [SerializeField] private string enemyHPScalingTablePath = "EnemyHPScaling";
    
    IReadOnlyDictionary<EnemyID ,EnemyData> enemyDatas;
    IReadOnlyList<EnemyHPScalingData> enemyHPScalingDatas;
    
    public IReadOnlyDictionary<EnemyID , EnemyData> EnemyDatas
    {
        get
        {
            if (enemyDatas == null)
            {
                throw new InvalidOperationException("Records not initialized. Call Load() first.");
            }
            return enemyDatas;
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        if (string.IsNullOrEmpty(enemyDataTablePath))
        {
            throw new ArgumentNullException(nameof(enemyDataTablePath), "Resource path cannot be null or empty.");
        }
        enemyDatas = TSVLoader.LoadTableToDictionary<EnemyID, EnemyData>(enemyDataTablePath, data => data.Enemy_ID );
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

public class EnemyDataManager : MonoSingleton<EnemyDataManager>
{
    [SerializeField] private string resourcePath = "EnemyData";
    
    IReadOnlyDictionary<EnemyID ,EnemyData> records;
    public IReadOnlyDictionary<EnemyID , EnemyData> Records
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
        if (string.IsNullOrEmpty(resourcePath))
        {
            throw new ArgumentNullException(nameof(resourcePath), "Resource path cannot be null or empty.");
        }
        records = TSVLoader.LoadTableToDictionary<EnemyID, EnemyData>(resourcePath, data => data.Enemy_ID );
        
    }
}
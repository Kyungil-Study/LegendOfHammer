

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StageDataManager : SingletonBase<StageDataManager>
{
    [SerializeField] private string resourcePath = "StageData";
    private List<StageWave> records = new();
    public IReadOnlyList<StageWave> Records
    {
        get
        {
            if (records == null)
            {
                throw new InvalidOperationException("Records not initialized. Call LoadAsync() first.");
            }
            return records;
        }
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        if (string.IsNullOrEmpty(resourcePath))
        {
            throw new ArgumentNullException(nameof(resourcePath), "Resource path cannot be null or empty.");
        }
        records = TSVLoader.LoadTable<StageWave>(resourcePath);
    }

}
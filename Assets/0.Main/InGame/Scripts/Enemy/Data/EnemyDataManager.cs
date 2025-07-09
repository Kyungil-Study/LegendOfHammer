using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

public class EnemyDataManager : MonoBehaviour
{
    public static EnemyDataManager Instance { get; private set; }
    public Dictionary<int, EnemyData> Records { get; private set; }

    async void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        await LoadTable();
    }

    private async Task LoadTable()
    {
        var list = await TSVLoader.LoadTableAsync<EnemyData>("EnemyUnitType", true);
        if (list == null)
        {
            Debug.LogError("[EnemyDataManager] 데이터 로딩 실패!");
            return;
        }

        foreach (var it in list)
        {
            Debug.Log($"{it.Atk_Power} {it.Movement_Pattern}" );
            
        }
        //Records = list.ToDictionary(recode => recode.EnemyID);
        Debug.Log($"[EnemyDataManager] Loaded {Records.Count} records");
    }
}
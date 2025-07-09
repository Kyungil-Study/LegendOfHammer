using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

public class EnemyDataManager : MonoBehaviour
{
    public static EnemyDataManager Instance { get; private set; }
    public Dictionary<EnemyID, EnemyData> Records { get; private set; }

    async void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        await LoadTable();
    }

    private async Task LoadTable()
    {
        var list = await TSVLoader.LoadTableAsync<EnemyData>("EnemyData", true);
        
        if (list == null)
        {
            Debug.LogError("[EnemyData] 데이터 로드 실패");
            return;
        }

        foreach (var it in list)
        {
            Debug.Log($"{it.Is_Ranged} // {it.Is_Ranged == true} // {it.EnemyMovementPattern}" );
        }
        Records = list.ToDictionary(recode => recode.Enemy_ID);
        Debug.Log($"[EnemyData] 데이터 로드 성공 // 모든 행 개수 : {Records.Count}");
    }
}
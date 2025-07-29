using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnDetector : MonoBehaviour
{
    [SerializeField] private MonsterHPUIManager hpUIManager;

    private readonly HashSet<MonoBehaviour> tracked = new();

    void Update()
    {
        // var monsters = FindObjectsOfType<MonoBehaviour>();
        //
        // foreach (var mb in monsters)
        // {
        //     if (mb.GetType().Name == "Monster" && tracked.Contains(mb) == false)
        //     {
        //         tracked.Add(mb);
        //         hpUIManager.RegisterMonster(mb);
        //     }
        // }
        var monsters = FindObjectsOfType<MonoBehaviour>();

        foreach (var mb in monsters)
        {
            if (mb.GetType().Name == "Monster" && tracked.Contains(mb) == false)
            {
                // EnemyID 및 Rank 확인
                var idProp = mb.GetType().GetProperty("EnemyID");
                if (idProp == null) continue;

                var value = idProp.GetValue(mb);
                if (value is EnemyID enemyID)
                {
                    if (EnemyDataManager.Instance.EnemyDatas.TryGetValue(enemyID, out var data))
                    {
                        if (data.Enemy_Rank == EnemyRank.Elite || data.Enemy_Rank == EnemyRank.Boss)
                        {
                            tracked.Add(mb);
                            hpUIManager.RegisterMonster(mb);
                        }
                    }
                }
            }
        }

    }
}
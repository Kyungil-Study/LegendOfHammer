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
    
    // HP 스케일링 테이블 가져오는 건데, HP 스케일링 수치를 테이블 연동으로 하기보다 그냥 인스펙터에서 열어두는게 좋아보임
    // 단순히, 1 ~ 10 스테이지는 1.12, 11 ~ 20 스테이지는 1.2, 21 ~ 30부터 1.25 곱하고
    // 이후부터는 몬스터별로 분기하는, 노말 몬스터라면 고정된 값 20000, 엘리트라면 50000, 보스라면 1000000 더하는 게 다라서
    // 사실상 의미있는 숫자는 1.12, 1.2, 1.25, 20000, 50000, 1000000 임
    
    public IReadOnlyList<EnemyHPScalingData> EnemyHPScalingDatas
    {
        get
        {
            if (enemyHPScalingDatas == null)
            {
                throw new InvalidOperationException("Records not initialized. Call Load() first.");
            }
            return enemyHPScalingDatas;
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
        enemyHPScalingDatas = TSVLoader.LoadTable<EnemyHPScalingData>(enemyHPScalingTablePath);
    }
}
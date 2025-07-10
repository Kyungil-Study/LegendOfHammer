using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MonsterSpawner : MonoSingleton<MonsterSpawner>
{
    // todo: 스테이지별 스폰 가능 몬스터 달라짐 , 데이터 테이블 연동 필요
    [FormerlySerializedAs("spawnPatternTable")] [FormerlySerializedAs("waveTable")] [SerializeField] private SpawnPatternTableSAO spawnPatternTableSao;
    
    [SerializeField] Monster[] monsterPrefabs; 
    [SerializeField] Monster monsterPrefab;
    
    [SerializeField] GameObject TestPlayer; // For testing purposes, remove later
    [Header("Spawn Points Reference")]
    [SerializeField] private Transform[] spawnPoints; // Maximum number of monsters allowed on screen
    [SerializeField] private Transform midSpawnPoints; // Maximum number of monsters allowed on screen
    // Start is called before the first frame update
    
    // todo : 테스트용 플레이어 참조, 나중에 지우기 For testing purposes, remove later
    [SerializeField] Monster TestEnemyPrefab; 
    List<StageWave> stageWaves = new List<StageWave>();
    Queue<StageWave> stageWavesQueue = new Queue<StageWave>();
    
    private void Awake()
    {
        var callbacks = BattleEventManager.Instance.Callbacks;
        callbacks.OnStartBattle += StartGame;
        callbacks.OnEndBattle += EndGame;
        spawnPatternTableSao.Resolve();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 테스트용, 나중에 지우기
            Monster testMonster = Instantiate(TestEnemyPrefab, spawnPoints[0]);
            testMonster.GetComponent<Monster>().SetPlayer(TestPlayer);
        }
    }

    private void EndGame(EndBattleEventArgs args)
    {
       
    }


    void StartGame(StartBattleEventArgs args)
    {
        
        
        
    }

    void SpawnPattern(SpawnPattern pattern)
    {
        Debug.Log($"pattern: {pattern.PatternType} , OnlyMid: {pattern.OnlyMid}");
        Transform spawnPoint;
        if (pattern.OnlyMid)
        {
            spawnPoint = midSpawnPoints;
        }
        else
        {
            int randomSpawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
            spawnPoint = spawnPoints[randomSpawnIndex];
        }

        foreach (SpawnPatternSlot slot in pattern.PatternSlots)
        {
            var spawnPosition = slot.transform.localPosition + spawnPoint.position;
            SpawnMonster(slot.SpawnRankType, slot.SpawnAttackType, spawnPosition);
        }
    }

    void SpawnMonster(EnemySpawnRankType spawnRankType, EnemySpawnAttackType spawnAttackType, Vector3 position)
    {
        int stageIndex = BattleManager.Instance.StageIndex;
        bool isRanged = spawnAttackType.Equals(EnemySpawnAttackType.Range);
        EnemyRank rank = spawnRankType.ToEnemyRank();

        var records = EnemyDataManager.Instance.Records;
        List<EnemyData> filteredRecords = new List<EnemyData>();
        foreach (var record in records.Values)
        {
            bool filtered = true;
            filtered &= record.Enemy_Rank.Equals(rank);
            filtered &= record.First_Appear_Stage <= stageIndex;
            filtered &= record.Is_Ranged.Equals(isRanged);
            if(filtered)
            {
                filteredRecords.Add(record);
            }
        }
        
        // todo: 몬스터 ID 세팅을 prefab 변경으로 수정
        Monster newMonster = Instantiate(TestEnemyPrefab, position, Quaternion.identity);
        newMonster.EnemyID = filteredRecords[UnityEngine.Random.Range(0, filteredRecords.Count)].Enemy_ID;
        newMonster.GetComponent<Monster>().SetPlayer(TestPlayer);
        
        // Monster newMonster = Instantiate(TestEnemyPrefab, position, Quaternion.identity);
        // todo: 테스트용, 병합 시 각주처리 제거하고 EnemyID 맞게 Instantiate하기 !!
        // newMonster.EnemyID = filteredRecords[UnityEngine.Random.Range(0, filteredRecords.Count)].Enemy_ID;
        // newMonster.GetComponent<Monster>().SetPlayer(TestPlayer);   // For testing purposes, remove later
      
        
    }
    
    void SpawnNormalPatternMonster()
    {
        // 테스트용
        var normalPatternList = new EnemySpawnPatternType[]
        {
            EnemySpawnPatternType.Normal_Three_A,
        };
        int randomIndex = UnityEngine.Random.Range(0, normalPatternList.Length);
        SpawnPattern spawnPattern = spawnPatternTableSao.GetSpawnPattern(normalPatternList[randomIndex]);
        SpawnPattern(spawnPattern);
        
        
    }
    
    void SpawnElitePatternMonster()
    {
        // 테스트용
        var elitePatternList = new EnemySpawnPatternType[]
        {
            EnemySpawnPatternType.Elite_One_A,
            EnemySpawnPatternType.Elite_Two_A,
            EnemySpawnPatternType.Elite_Three_A,
        };
        int randomIndex = UnityEngine.Random.Range(0, elitePatternList.Length);
        SpawnPattern spawnPattern = spawnPatternTableSao.GetSpawnPattern(elitePatternList[randomIndex]);
        SpawnPattern(spawnPattern);
    }
    
    void SpawnBossPatternMonster()
    {
        // 테스트용
        var bossPatternList = new EnemySpawnPatternType[]
        {
            EnemySpawnPatternType.Boss_One_A,
        };
        int randomIndex = UnityEngine.Random.Range(0, bossPatternList.Length);
        SpawnPattern spawnPattern = spawnPatternTableSao.GetSpawnPattern(bossPatternList[randomIndex]);
        SpawnPattern(spawnPattern);
    }

    public void SpawnMonster(WaveRankType currentWaveWaveRank)
    {
        switch (currentWaveWaveRank)
        {
            case WaveRankType.Normal:
                SpawnNormalPatternMonster();
                break;
            case WaveRankType.Elite:
                SpawnElitePatternMonster();
                break;
            case WaveRankType.Boss:
                SpawnBossPatternMonster();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentWaveWaveRank), currentWaveWaveRank, null);
            
        }
    }
}

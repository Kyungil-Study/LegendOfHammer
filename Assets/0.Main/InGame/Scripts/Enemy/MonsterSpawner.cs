using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MonsterSpawner : MonoSingleton<MonsterSpawner>
{
    // todo: 스테이지별 스폰 가능 몬스터 달라짐 , 데이터 테이블 연동 필요
    [FormerlySerializedAs("spawnPatternTable")] [FormerlySerializedAs("waveTable")] [SerializeField] private SpawnPatternTableSAO spawnPatternTableSao;
    
    [SerializeField] Monster monsterPrefab;
    [SerializeField] EliteMonster eliteMonsterPrefab;
    [SerializeField] Boss bossMonsterPrefab;
    
    [SerializeField] GameObject TestPlayer; // For testing purposes, remove later
    [Header("Spawn Points Reference")]
    [SerializeField] private Transform[] spawnPoints; // Maximum number of monsters allowed on screen
    [SerializeField] private Transform midSpawnPoints; // Maximum number of monsters allowed on screen
    // Start is called before the first frame update
    
    List<StageWave> stageWaves = new List<StageWave>();
    Queue<StageWave> stageWavesQueue = new Queue<StageWave>();
    
    private void Awake()
    {
        var callbacks = BattleEventManager.Instance.Callbacks;
        callbacks.OnStartBattle += StartGame;
        callbacks.OnEndBattle += EndGame;
        spawnPatternTableSao.Resolve();
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
            SpawnMonster(slot.RankType, slot.AttackType, spawnPosition);
        }
    }

    void SpawnMonster(EnemyRankType rankType, EnemyAttackType attackType, Vector3 position)
    {
        switch (rankType)
        {
            case EnemyRankType.Normal:
                Monster newMonster = Instantiate(monsterPrefab, position, Quaternion.identity);
                newMonster.GetComponent<Monster>().SetPlayer(TestPlayer);   // For testing purposes, remove later
                break;
            case EnemyRankType.Elite:
                EliteMonster newEliteMonster = Instantiate(eliteMonsterPrefab, position, Quaternion.identity);
                break;
            case EnemyRankType.Boss:
                Boss newBossMonster = Instantiate(bossMonsterPrefab, position, Quaternion.identity);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(rankType), rankType, null);
            
        }
        
    }
    
    void SpawnNormalPatternMonster()
    {
        // 테스트용
        var normalPatternList = new EnemySpawnPatternType[]
        {
            EnemySpawnPatternType.Normal_Three_A,
            EnemySpawnPatternType.Normal_Three_B,
            EnemySpawnPatternType.Normal_Three_C,
            EnemySpawnPatternType.Normal_Three_D,
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
            EnemySpawnPatternType.Elite_One_B,
            EnemySpawnPatternType.Elite_Two_A,
            EnemySpawnPatternType.Elite_Two_B,
            EnemySpawnPatternType.Elite_Two_C,
            EnemySpawnPatternType.Elite_Three_A,
            EnemySpawnPatternType.Elite_Three_B,
            EnemySpawnPatternType.Elite_Three_C,
            EnemySpawnPatternType.Elite_Three_D,
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
            EnemySpawnPatternType.Boss_One_B,
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

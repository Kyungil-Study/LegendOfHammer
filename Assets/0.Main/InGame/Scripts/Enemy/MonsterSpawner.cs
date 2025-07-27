using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoSingleton<MonsterSpawner>
{
    [SerializeField] private EnemyID testSpawnID = EnemyID.Straight_Normal_001; // For testing purposes, remove later
    
    public class PatternActivation
    {
        public int ID { get; set; }
        public EnemySpawnPatternType PatternName { get; set; }
        public WaveRankType Rank { get; set; }
        public bool OnlyMid { get; set; }
        public int NormalMelee { get; set; }
        public int NormalRange { get; set; }
        public int EliteMelee { get; set; }
        public int EliteRange { get; set; }
        public int BossMelee { get; set; }
        public int BossRange { get; set; }
        public int AppearStage { get; set; }
        public int DisappearStage { get; set; }
    }
    
    // todo: 스테이지별 스폰 가능 몬스터 달라짐 , 데이터 테이블 연동 필요
    [FormerlySerializedAs("spawnPatternTable")] [FormerlySerializedAs("waveTable")] [SerializeField] private SpawnPatternTableSAO spawnPatternTableSao;
    
    [SerializeField] Monster[] monsterPrefabs; 
    [SerializeField] Monster monsterPrefab;
    
    [SerializeField] GameObject player;
    [Header("Spawn Points Reference")]
    [SerializeField] private Transform[] spawnPoints; // Maximum number of monsters allowed on screen
    [SerializeField] private Transform midSpawnPoints; // Maximum number of monsters allowed on screen
    // Start is called before the first frame update

    [SerializeField] bool isTestMode = false; // For testing purposes, remove later
    [SerializeField] Monster TestEnemyPrefab; // todo : 테스트용, 나중에 지우기 For testing purposes, remove later

        
    Dictionary<WaveRankType, List<EnemySpawnPatternType>> activatedPatternByRank = new Dictionary<WaveRankType, List<EnemySpawnPatternType>>()
    {
        { WaveRankType.Normal, new List<EnemySpawnPatternType>() },
        { WaveRankType.Elite, new List<EnemySpawnPatternType>() },
        { WaveRankType.Boss, new List<EnemySpawnPatternType>() }
    };

    
    private void Awake()
    {
        BattleEventManager.RegistEvent<StartBattleEventArgs>(StartGame);
        BattleEventManager.RegistEvent<EndBattleEventArgs>(EndGame);
        spawnPatternTableSao.Resolve();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 테스트용, 나중에 지우기
            var monster = Instantiate(TestEnemyPrefab, spawnPoints[2].position, Quaternion.identity);
            monster.SetEnemyID(testSpawnID);
            monster.SetPlayer(player);
        }
    }

    private void EndGame(EndBattleEventArgs args)
    {
       
    }


    void StartGame(StartBattleEventArgs args)
    {
        int stageIndex = args.StageIndex;

        var datas = PatternActivationDataManager.Instance.Records;
        foreach (var activation in datas)
        {
            if (activation.AppearStage <= stageIndex && stageIndex <= activation.DisappearStage)
            {
                activatedPatternByRank[activation.Rank].Add(activation.PatternName);
            }
        }
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
        Debug.Log($"[MonsterSpawner] Spawned {filteredRecords.Count} monsters of rank {rank} and attack type {spawnAttackType} at position {position}");
        // todo: 몬스터 ID 세팅을 prefab 변경으로 수정
        Monster newMonster = Instantiate(monsterPrefab, position, Quaternion.identity);
        EnemyID pickedId = filteredRecords[Random.Range(0, filteredRecords.Count)].Enemy_ID;
        newMonster.SetEnemyID(pickedId);
        newMonster.SetPlayer(player);
    }

    public void SpawnMonster(WaveRankType currentWaveWaveRank)
    {
        var patternList = activatedPatternByRank[currentWaveWaveRank];
        int randomIndex = UnityEngine.Random.Range(0, patternList.Count);
        SpawnPattern spawnPattern = spawnPatternTableSao.GetSpawnPattern(patternList[randomIndex]);
        SpawnPattern(spawnPattern);
    }
}

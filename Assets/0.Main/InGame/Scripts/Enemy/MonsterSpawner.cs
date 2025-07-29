using System;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private SpawnPatternTableSAO spawnPatternTableSao;
    
    [SerializeField] Monster[] monsterPrefabs; 
    [SerializeField] Monster monsterPrefab;
    
    [SerializeField] GameObject player;
    [Header("Spawn Points Reference")]
    [SerializeField] private Transform[] spawnPoints; // Maximum number of monsters allowed on screen
    [SerializeField] private Transform midSpawnPoints; // Maximum number of monsters allowed on screen
    // Start is called before the first frame update

    [SerializeField] bool isTestMode = false; // For testing purposes, remove later
    [SerializeField] Monster TestEnemyPrefab; // todo : 테스트용, 나중에 지우기 For testing purposes, remove later

        
    private Dictionary<KeyValuePair<EnemySpawnRankType, EnemySpawnAttackType>, EnemyData> cachedEnemies
        = new();
    private int stageIndex;

    
    private void Awake()
    {
        BattleEventManager.RegistEvent<StartBattleEventArgs>(StartGame);
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

    void StartGame(StartBattleEventArgs args)
    {
        this.stageIndex = args.StageIndex;
    }

    void SpawnPattern(SpawnPattern pattern)
    {
        try
        {
            cachedEnemies.Clear();;
            Debug.Log($"pattern: {pattern.PatternType} , OnlyMid: {pattern.OnlyMid}");
            Transform spawnPoint;
            if (pattern.OnlyMid)
            {
                spawnPoint = midSpawnPoints;
            }
            else
            {
                if (spawnPoints.Length == 0)
                {
                    Debug.LogError("[MonsterSpawner] No spawn points available.");
                    return;
                }
                int randomSpawnIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
                spawnPoint = spawnPoints[randomSpawnIndex];
            }

            foreach (SpawnPatternSlot slot in pattern.PatternSlots)
            {
                var spawnPosition = slot.transform.localPosition + spawnPoint.position;
                var enemyData = GetFilteredRandomEnemyData(slot.SpawnRankType, slot.SpawnAttackType);
                SpawnMonster(enemyData , spawnPosition);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[MonsterSpawner] Error while spawning pattern: {e.Message}");
            return;
        }
    }

    
    EnemyData GetFilteredRandomEnemyData(EnemySpawnRankType spawnRankType, EnemySpawnAttackType spawnAttackType)
    {
        
        EnemyData GotchaMonsterData(IReadOnlyList<EnemyData> datas)
        {
            if (datas == null || datas.Count == 0)
            {
                Debug.LogWarning($"[MonsterSpawner] No enemies found for {spawnRankType} and {spawnAttackType}");
                return null;
            }
            // Return a random enemy from the cached data
            int randomIndex = Random.Range(0, datas.Count);
            return datas[randomIndex];
            
        }
        
        var key = new KeyValuePair<EnemySpawnRankType, EnemySpawnAttackType>(spawnRankType, spawnAttackType);
        if (cachedEnemies.TryGetValue(key, out var cachedData))
        {
            return cachedData;
        }
        else
        {
            bool isRanged = spawnAttackType.Equals(EnemySpawnAttackType.Range);
            EnemyRank rank = spawnRankType.ToEnemyRank();
            var records = EnemyDataManager.Instance.EnemyDatas;
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
            
            cachedEnemies[key] = GotchaMonsterData(filteredRecords);
            return cachedEnemies[key];
        }
       
    }
    

    void SpawnMonster(EnemyData data, Vector3 position)
    {
        if (data.Enemy_Rank.Equals(EnemyRank.Boss))
        {
            BattlePopupSystem.Instance.BossAlarm.ExecuteAlarm();
        }
        
        Debug.Log($"[MonsterSpawner] Spawned monsters of rank {data.Enemy_Rank} and attack type {data.Atk_Pattern} at position {position}");
        // todo: 몬스터 ID 세팅을 prefab 변경으로 수정
        Monster newMonster = Instantiate(monsterPrefab, position, Quaternion.identity);
        newMonster.SetEnemyID(data.Enemy_ID);
        newMonster.SetPlayer(player);
    }

    public void SpawnMonster(WaveRankType currentWaveWaveRank)
    {
        Debug.Log($"[MonsterSpawner] SpawnMonster called with rank: {currentWaveWaveRank}");
        var patternList = spawnPatternTableSao.FilteredSpawnPatterns(stageIndex, currentWaveWaveRank);
        SpawnPattern spawnPattern = patternList[Random.Range(0, patternList.Count)];
        SpawnPattern(spawnPattern);
    }
}

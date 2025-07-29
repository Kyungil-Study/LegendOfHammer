using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MonsterTester : MonoBehaviour
{
    [Header("프리팹 & 참조")]
    [SerializeField] private Monster monsterPrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPoint;

    [Header("플레이어 이동속도")]
    [SerializeField] private float playerSpeed = 5f;
    
    [Header("단일 스폰 (Space)")]
    [SerializeField] private EnemyID testEnemyID = EnemyID.Straight_Normal_001;
    
    [Header("패턴 스폰 (테스트모드 체크 + 1번 키)")]
    [SerializeField] private EnemyID HPTestRankID = EnemyID.Straight_Normal_001;
    [SerializeField] private EnemyMovementPattern testMovePattern = EnemyMovementPattern.Straight;
    [SerializeField] private EnemyAttackPattern testAttackPattern = EnemyAttackPattern.Normal;
    
    [Header("HP 스케일링 확인")]
    [SerializeField] private int stageIndex = 1; // 스테이지 인덱스, 테스트용
    [SerializeField] private TextMeshProUGUI monsterInfoText;
    
    [Space(10),Header("웨이브 세팅")]
    [LabelText("자동 웨이브 스폰 여부"), SerializeField] private bool autoWaveSpawnMode = false; // 자동 웨이브 스폰 여부
    [LabelText("자동 웨이브 스폰 주기"), SerializeField] private float autoWaveSpawnInterval = 15; // 웨이브 인덱스, 테스트용
    private float autoWaveSpawnTimer = 0f; // 자동 웨이브 스폰 타이머
    [SerializeField] private SpawnPattern spawnPatternTestSample;
    [SerializeField] private EnemyID normalMeleeID = EnemyID.Straight_Normal_001;
    [SerializeField] private EnemyID normalRangeID = EnemyID.Zigzag_Normal_002;
    [SerializeField] private EnemyID eliteMeleeID = EnemyID.Straight_Elite_001;
    [SerializeField] private EnemyID eliteRangeID = EnemyID.Zigzag_Elite_002;
    [SerializeField] private EnemyID bossMeleeID = EnemyID.Straight_Boss_001;
    [SerializeField] private EnemyID bossRangeID = EnemyID.Zigzag_Boss_002;
    
    private Camera cam;

    private Monster              spawnedMonster;
    private EnemyRank            monsterRank;
    private EnemyMovementPattern monsterMovePattern;
    private EnemyAttackPattern   monsterAttackPattern;
    
    private void Awake()
    {
        cam = Camera.main;
    }
    
    void Update()
    {
        HandlePlayerMovement();
        HandleMouseClick();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnSingle();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnByPattern();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            for (int i = 0; i < EnemyDataManager.Instance.EnemyHPScalingDatas.Count; i++)
            {
                var enemyData = EnemyDataManager.Instance.EnemyHPScalingDatas[i];
                Debug.Log($"HP 스케일링 테이블 연동 여부 확인 : {enemyData.Stage} / {enemyData.HP_Scaling}");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnByWavePattern();
        }

        if (autoWaveSpawnMode)
        {
            autoWaveSpawnTimer += Time.deltaTime;
            if (autoWaveSpawnTimer >= autoWaveSpawnInterval)
            {
                autoWaveSpawnTimer = 0f;
                SpawnByWavePattern();
            }
            
        }
    }

    private void SpawnByWavePattern()
    {
        spawnPatternTestSample.ResolveWaveSlots();
        if (spawnPatternTestSample == null)
        {
            Debug.LogError("[MonsterTester] SpawnPattern is null. Please assign a valid SpawnPattern.");
            return;
        }

        foreach (var slots in spawnPatternTestSample.PatternSlots)
        {
            if (slots == null)
            {
                Debug.LogError("[MonsterTester] SpawnPatternSlot is null. Please check the SpawnPattern.");
                continue;
            }

            Vector3 spawnPosition = slots.transform.position + spawnPoint.position;
                
            EnemyID enemyID = slots.SpawnRankType switch
            {
                EnemySpawnRankType.Normal => slots.SpawnAttackType == EnemySpawnAttackType.Melee ? normalMeleeID : normalRangeID,
                EnemySpawnRankType.Elite  => slots.SpawnAttackType == EnemySpawnAttackType.Melee ? eliteMeleeID : eliteRangeID,
                EnemySpawnRankType.Boss   => slots.SpawnAttackType == EnemySpawnAttackType.Melee ? bossMeleeID : bossRangeID,
                _                          => normalMeleeID // 기본값 설정
            };
                
            var monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
            monster.SetEnemyID(enemyID);
            monster.SetPlayer(player);
        }
    }

    private void LateUpdate()
    {
        if (monsterInfoText == null) return;

        var sb = new StringBuilder();

        sb.AppendLine("스폰한 몬스터 정보");
        sb.AppendLine();

        if (spawnedMonster != null)
        {
            sb.AppendLine($"Enemy ID           : {spawnedMonster.EnemyID}");
            sb.AppendLine($"Enemy Rank       : {monsterRank}");
            sb.AppendLine($"이동 패턴           : {monsterMovePattern}");
            sb.AppendLine($"공격 패턴           : {monsterAttackPattern}");
            sb.AppendLine($"현재 스테이지        : {stageIndex}");
            sb.AppendLine($"HP : {spawnedMonster.Stat.CurrentHP} / {spawnedMonster.Stat.FinalStat.HP}");
        }
        else
        {
            sb.AppendLine("아직 스폰된 몬스터가 없습니다.");
        }

        monsterInfoText.text = sb.ToString();
    }

    private void HandlePlayerMovement()
    {
        float dx = 0f, dy = 0f;
        
        if (Input.GetKey(KeyCode.W)) dy =  1f;
        if (Input.GetKey(KeyCode.S)) dy = -1f;
        if (Input.GetKey(KeyCode.D)) dx =  1f;
        if (Input.GetKey(KeyCode.A)) dx = -1f;

        Vector3 dir = new Vector3(dx, dy, 0f);
        
        if (dir.sqrMagnitude > 0.01f)
        {
            dir.Normalize();
            player.transform.position += dir * (playerSpeed * Time.deltaTime);
        }
    }
    
    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0) == false) return;
        if (cam == null) return;

        Vector3 screenPos = Input.mousePosition;
        Vector3 worldPos  = cam.ScreenToWorldPoint(screenPos);
        
        worldPos.z = spawnPoint.position.z;
        spawnPoint.position = worldPos;
    }
    
    void SpawnSingle()
    {
        var monster = Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);
        monster.IsTestMode = false;
        
        monster.SetEnemyID(testEnemyID);
        monster.SetPlayer(player);
        
        spawnedMonster       = monster;
        var data          = EnemyDataManager.Instance.EnemyDatas[testEnemyID];
        monsterRank          = data.Enemy_Rank;
        monsterMovePattern   = data.EnemyMovementPattern;
        monsterAttackPattern = data.Atk_Pattern;
    }

    private void SpawnByPattern()
    {
        var monster = Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);
        monster.SetEnemyID(HPTestRankID);

        if (EnemyDataManager.Instance.EnemyDatas.TryGetValue(HPTestRankID, out var data))
        {
            monster.Stat.Initialize(data, stageIndex);
            monsterRank = data.Enemy_Rank;
        }
        else
        {
            Debug.LogWarning($"[MonsterTest] EnemyID {HPTestRankID} not in table, using defaults");
        }

        monster.SetPlayer(player);
        monster.IsTestMode = true;
        monster.MonsterTest(testMovePattern, testAttackPattern);
        
        spawnedMonster       = monster;
        monsterMovePattern   = testMovePattern;
        monsterAttackPattern = testAttackPattern;
    }
}

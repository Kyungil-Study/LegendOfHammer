using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
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
    [SerializeField] private EnemyID  patternDefaultID    = EnemyID.Straight_Normal_001;
    [SerializeField] private EnemyMovementPattern testMovePattern = EnemyMovementPattern.Straight;
    [SerializeField] private EnemyAttackPattern   testAttackPattern = EnemyAttackPattern.Normal;
    
    private Camera cam;
    
    private Monster              lastSpawned;
    private EnemyRank            lastRank;
    private EnemyMovementPattern lastMovePattern;
    private EnemyAttackPattern   lastAttackPattern;
    
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
        
        lastSpawned       = monster;
        var data          = EnemyDataManager.Instance.EnemyDatas[testEnemyID];
        lastRank          = data.Enemy_Rank;
        lastMovePattern   = data.EnemyMovementPattern;
        lastAttackPattern = data.Atk_Pattern;
    }

    private void SpawnByPattern()
    {
        var monster = Instantiate(monsterPrefab, spawnPoint.position, Quaternion.identity);
        monster.SetEnemyID(patternDefaultID);

        if (EnemyDataManager.Instance.EnemyDatas.TryGetValue(patternDefaultID, out var data))
        {
            monster.Stat.Initialize(data, BattleManager.Instance.StageIndex);
            lastRank          = data.Enemy_Rank;
        }
        else
        {
            Debug.LogWarning($"[MonsterTest] EnemyID {patternDefaultID} not in table, using defaults");
        }

        monster.SetPlayer(player);
        monster.IsTestMode = true;
        monster.MonsterTest(testMovePattern, testAttackPattern);
        
        lastSpawned       = monster;
        lastMovePattern   = testMovePattern;
        lastAttackPattern = testAttackPattern;
    }

    void OnGUI()
    {
        const float width  = 260;
        const float height = 200;
        const float margin = 10;

        float x = margin;
        float y = Screen.height - height - margin;

        GUILayout.BeginArea(new Rect(x, y, width, height), "Monster Test Info", GUI.skin.window);

        GUILayout.Label("마우스 클릭한 위치에 스폰 + WASD 이동 가능");
        GUILayout.Label("테이블 기반 스폰 : Space키 \n패턴별 스폰 : 1번키 ");
        GUILayout.Space(8);

        if (lastSpawned != null)
        {
            GUILayout.Label($"Enemy ID           : {lastSpawned.EnemyID}");
            GUILayout.Label($"Enemy Rank         : {lastRank}");
            GUILayout.Label($"Movement Pattern   : {lastMovePattern}");
            GUILayout.Label($"Attack Pattern     : {lastAttackPattern}");
            GUILayout.Space(4);

            // var s = lastSpawned.Stat.FinalStat;
            // GUILayout.Label($"Stat - HP          : {s.HP}");
            // GUILayout.Label($"Stat - Atk         : {s.Atk}");
            // GUILayout.Label($"Stat - MoveSpeed   : {s.MoveSpeed:F2}");
        }
        else
        {
            GUILayout.Label("아직 스폰된 몬스터가 없습니다.");
        }

        GUILayout.EndArea();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BattleManager : MonoSingleton<BattleManager>
{
    public int StageIndex = 0;
    public int MaxStageNumber = 0;
    
    [Header("추격 게이지 세팅")]
    [SerializeField] private float chaseGuageDecreaseRate = 0.5f; // Increase rate per second
    [SerializeField] private float chaseIncreaseRate = 1f; // Increase rate when monster is through clear zone
    [SerializeField] private float chaseGuageMax = 100f; // Maximum value for chase gauge
    private ClampedFloat chaseGuage;
    public ClampedFloat ChaseGuage => chaseGuage;

    private bool isEnded = false;

    protected override void Initialize()
    {
        Debug.Log("[BattleManager] Initialize called.");
        base.Initialize();
        
        // Initialize chase gauge
        chaseGuage = new ClampedFloat(0f, chaseGuageMax, 0f);
        
        // Register event listeners
        BattleEventManager.Instance.Callbacks.OnAliveMonster += OnAliveMonster;
        BattleEventManager.Instance.Callbacks.OnDeath += OnDeath;
        chaseGuage.Events.OnMaxReached += (cur, max) =>
        {
            Debug.Log("[BattleManager] Chase gauge reached maximum value. Ending game.");
            EndGame(false, false); // Game over if chase gauge is full
        };
    }
    

    private void OnDeath(DeathEventArgs args)
    {
        Debug.Log($"[BattleManager] OnDeath called.");
        if (args.Target as Squad)
        {
            Debug.Log("Player has died. Ending game.");
            EndGame(false,false);
        }
        else if(args.Target is Monster monster)
        {
            Debug.Log($"[BattleManager] Monster {monster.EnemyID} has died.");
            var id = monster.EnemyID;
            var data = EnemyDataManager.Instance.Records[id];
            if( data.Enemy_Rank.Equals(EnemyRank.Boss))
            {
                Debug.Log($"Boss Monster has died.");
                EndGame(true, true);
            }
        }
    }

    private void OnAliveMonster(AliveMonsterEventArgs args)
    {
        var monster = args.Monster as Monster;
        Debug.Log($"[BattleManager] Monster {monster.EnemyID} is alive.");
        var data = EnemyDataManager.Instance.Records[monster.EnemyID];
        if (data.Enemy_Rank.Equals(EnemyRank.Boss))
        {
            EndGame(true, false);
        }
        // todo: UI 완료되면 활성화
        var enemyData = EnemyDataManager.Instance.Records[monster.EnemyID];
        chaseGuage.Increase(enemyData.Chasing_Increase);
    }


    public void ReadyGame()
    {
        Debug.Log("[BattleManager] ReadyGame called.");
        var es3Manager = ES3Manager.Instance;
        var stageData = es3Manager.StageData;
        
        StageIndex = stageData.CurrentStage;
        MaxStageNumber = stageData.MaxStage;
        
        BattleEventManager.Instance.CallEvent(new ReadyBattleEventArgs(
            stageIndex: StageIndex,
            maxStageIndex: MaxStageNumber));
    }

    public void StartGame() // todo: 로딩 연동 필요
    {
        Debug.Log($"[BattleManager] Starting game for stage {StageIndex}.");
        StartBattleEventArgs startEventArgs = new StartBattleEventArgs(StageIndex);
        BattleEventManager.Instance.CallEvent(startEventArgs);
    }

    private void Update()
    {
        if (isEnded)
        {
            return;
        }
        
        chaseGuage.Decrease(chaseGuageDecreaseRate * Time.deltaTime);
    }

    // Update is called once per frame
    void EndGame(bool isVictory,bool isBossDead)
    {
        isEnded = true;
        // Here you can handle the end of the game, such as showing a UI or transitioning to another scene
        Debug.Log(isVictory ? "Battle ended with victory!" : "Battle ended with defeat!");
        
        // Call the end battle event
        EndBattleEventArgs endEventArgs = new EndBattleEventArgs(isVictory,isBossDead); // Assuming victory for now
        BattleEventManager.Instance.CallEvent(endEventArgs);
    }
    
    private List<Monster> m_WholeMonsters = new List<Monster>();
    public IEnumerable<Monster> GetAllMonsters()
    {
        return m_WholeMonsters;
    }
    
    public void RegisterMonster(Monster monster)
    {
        if (m_WholeMonsters.Contains(monster) == false)
        {
            m_WholeMonsters.Add(monster);
        }
    }

    public void UnregisterMonster(Monster monster)
    {
        if (m_WholeMonsters.Contains(monster))
        {
            m_WholeMonsters.Remove(monster);
        }
    }
    
    public static bool TryGetMonsterBy(Collider2D collider ,out Monster monster)
    {
        return collider.TryGetComponent(out monster);
    }
    
    public static List<Monster> GetAllEnemyInRadius(Vector3 position, float radius)
    {
        var inRadius = Physics2D.OverlapCircleAll(position, radius, LayerMask.GetMask("Monster"));
        List<Monster> enemies = new List<Monster>();
        foreach (var collider in inRadius)
        {
            if (TryGetMonsterBy(collider, out Monster monster))
            {
                enemies.Add(monster);
            }
        }
        return enemies;
    }
    
    private static readonly Collider2D[] colliderBuffer = new Collider2D[64]; // 캐시된 배열

    public static void GetAllEnemyInRadiusNonAlloc(Vector3 position, float radius, List<Monster> result)
    {
        result.Clear(); // 결과 리스트 초기화

        int count = Physics2D.OverlapCircleNonAlloc(position, radius, colliderBuffer, LayerMask.GetMask("Monster"));
        for (int i = 0; i < count; i++)
        {
            if (TryGetMonsterBy(colliderBuffer[i], out Monster monster))
            {
                result.Add(monster);
            }
        }
    }

}

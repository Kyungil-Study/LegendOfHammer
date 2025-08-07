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
    [SerializeField] private float chaseGuageMax = 100f; // Maximum value for chase gauge
    private ClampedInt score = new ClampedInt(0, Int32.MaxValue, 0); // Clamped value for chase gauge max
    private ClampedFloat chaseGuage;
    public ClampedFloat ChaseGuage => chaseGuage;
    public int Score => score.Current;

    private bool isEnded = false;
    public bool IsEnded => isEnded;
    
    private bool isBossAlived = false;
    public bool IsBossAlived => isBossAlived;

    protected override void Initialize()
    {
        Debug.Log("[BattleManager] Initialize called.");
        base.Initialize();
        
        // Initialize chase gauge
        chaseGuage = new ClampedFloat(0f, chaseGuageMax, 0f);
        
        // Register event listeners
        BattleEventManager.RegistEvent<AliveMonsterEventArgs>(OnAliveMonster);
        BattleEventManager.RegistEvent<DeathEventArgs>(OnDeath);
        BattleEventManager.RegistEvent<PauseBattleEventArgs>(OnPauseBattle );
        chaseGuage.Events.OnMaxReached += (cur, max) =>
        {
            Debug.Log("[BattleManager] Chase gauge reached maximum value. Ending game.");
            EndGame(false, false); // Game over if chase gauge is full
        };
        
        StartCoroutine(ReadyGameCoroutine());

        
    }

    private void OnPauseBattle(PauseBattleEventArgs obj)
    {
        if (obj.IsPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
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
            var data = EnemyDataManager.Instance.EnemyDatas[id];
            if( data.Enemy_Rank.Equals(EnemyRank.Boss))
            {
                Debug.Log($"Boss Monster has died.");
                EndGame(true, true);
            }
            
            {
                var monsterBonus = data.Chasing_Increase;
                score.Increase(monsterBonus);
            }

        }
    }

    private void OnAliveMonster(AliveMonsterEventArgs args)
    {
        var monster = args.AliveMonster;
        Debug.Log($"[BattleManager] Monster {monster.EnemyID} is alive.");
        var enemyData = EnemyDataManager.Instance.EnemyDatas[monster.EnemyID];
        chaseGuage.Increase(enemyData.Chasing_Increase);    
        var data = EnemyDataManager.Instance.EnemyDatas[monster.EnemyID];
        if (data.Enemy_Rank.Equals(EnemyRank.Boss))
        {
            isBossAlived = true;
            EndGame(true, false);
        }
        
        
    }


    IEnumerator ReadyGameCoroutine()
    {
        yield return new WaitForEndOfFrame();
        
        Debug.Log("[BattleManager] ReadyGame called.");
        var es3Manager = ES3Manager.Instance;
        var stageData = es3Manager.StageData;
        ES3Manager.Instance.SetAttemptCount(stageData.StageAttemptCount + 1);
        StageIndex = stageData.CurrentStage;
        MaxStageNumber = stageData.MaxStage;
        
        BattleEventManager.CallEvent(new ReadyBattleEventArgs(
            stageIndex: StageIndex,
            maxStageIndex: MaxStageNumber));
        
        if (SessionManager.Instance.IsContinueGame)
        {
            if(AugmentInventory.Instance.IsFullClassAugment())
            {
                BattleUIController.Instance.SwapPage(UIPageType.CommonAugmentSelection);
            }
            else
            {
                BattleUIController.Instance.SwapPage( UIPageType.ClassAumgentSelection); // Assuming the next page is ClassAugmentSelection
            }
        }
        else
        {
            BattleUIController.Instance.SwapPage(UIPageType.LobbyPage);
        }
        
    }

    public void StartGame() // todo: 로딩 연동 필요
    {
        Debug.Log($"[BattleManager] Starting game for stage {StageIndex}.");
        StartBattleEventArgs startEventArgs = new StartBattleEventArgs(StageIndex);
        BattleEventManager.CallEvent(startEventArgs);
        SoundManager.Instance.PlayRandomGameBgm();
    }

    // Update is called once per frame
    void EndGame(bool isVictory,bool isBossDead)
    {
        if (isEnded)
        {
            return;
        }
        
        isEnded = true;
        // Here you can handle the end of the game, such as showing a UI or transitioning to another scene
        Debug.Log(isVictory ? "Battle ended with victory!" : "Battle ended with defeat!");
        
        // Call the end battle event
        EndBattleEventArgs endEventArgs = new EndBattleEventArgs(isVictory,isBossDead); // Assuming victory for now
        BattleEventManager.CallEvent(endEventArgs);
    }
    
    private List<Monster> m_WholeMonsters = new List<Monster>();
    public IEnumerable<Monster> GetAllMonsters()
    {
        return m_WholeMonsters;
    }
    
    public static void RegisterMonster(Monster monster)
    {
        if( Instance == null )
        {
            Debug.LogWarning("[BattleManager] Instance is null. Cannot register monster.");
            return;
        }
        
        if (Instance.m_WholeMonsters.Contains(monster) == false)
        {
            Instance.m_WholeMonsters.Add(monster);
        }
    }

    public static void UnregisterMonster(Monster monster)
    {
        if (Instance == null)
        {
            Debug.LogWarning("[BattleManager] Instance is null. Cannot unregister monster.");
            return;
        }
        
        if (Instance.m_WholeMonsters.Contains(monster))
        {
            Instance.m_WholeMonsters.Remove(monster);
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

    public void Revive()
    {
        isEnded = false;
        chaseGuage.ResetToMin();
        BattleEventManager.CallEvent<ReviveEventArgs>(new ReviveEventArgs());
    }
}

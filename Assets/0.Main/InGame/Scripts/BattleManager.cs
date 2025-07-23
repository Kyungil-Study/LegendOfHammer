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
    [SerializeField] GameObject loadUI;

    public int StageIndex = 0;
    public int MaxStageNumber = 0;
    
    [Header("추격 게이지 세팅")]
    [SerializeField] private float chaseGuageDecreaseRate = 0.5f; // Increase rate per second
    [SerializeField] private float chaseIncreaseRate = 1f; // Increase rate when monster is through clear zone
    [SerializeField] private float chaseGuageMax = 100f; // Maximum value for chase gauge]
    private float chaseGuage = 0f; // 0 to 100

    private bool isEnded = false;
    private void Awake()
    {
        BattleEventManager.Instance.Callbacks.OnAliveMonster += OnAliveMonster;
        BattleEventManager.Instance.Callbacks.OnDeath += OnDeath;
        
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
        /*chaseGuage += chaseIncreaseRate;
        if (chaseGuage >= chaseGuageMax)
        {
            EndGame(false);
        }*/
    }

    private void Start()
    {
        StartCoroutine(ReadyGame()); ;
    }

    private IEnumerator ReadyGame()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("[BattleManager] ReadyGame called.");

        loadUI.SetActive(false);
        if(BackendStageGameData.stage == null)
        {
            Debug.LogWarning("[BattleManager] BackendStageGameData.stage is null. Using default stage index 1.");
            MaxStageNumber = StageIndex;
        }
        else
        {
            StageIndex = BackendStageGameData.stage.Currentstage;; // For testing purposes, remove later
            MaxStageNumber = BackendStageGameData.stage.Maxstage;
        }
        
        
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
        
        chaseGuage -= chaseGuageDecreaseRate * Time.deltaTime;
        chaseGuage = Mathf.Clamp(chaseGuage, 0f, chaseGuageMax);
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
}

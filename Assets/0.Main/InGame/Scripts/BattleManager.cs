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
        Debug.Log("[BattleManager] OnDeath called.");
        if (args.Target as Squad)
        {
            Debug.Log("Player has died. Ending game.");
            EndGame(false);
        }
        else if(args.Target is Monster monster)
        {
            var id = monster.EnemyID;
            var data = EnemyDataManager.Instance.Records[id];
            if( data.Enemy_Rank.Equals(EnemySpawnRankType.Boss))
            {
                Debug.Log($"Boss Monster has died.");
                EndGame(true);
            }
        }
    }

    private void OnAliveMonster(AliveMonsterEventArgs args)
    {
        var monster = args.Monster as Monster;
        Debug.Log($"[BattleManager] Monster {monster.EnemyID} is alive.");
        var data = EnemyDataManager.Instance.Records[monster.EnemyID];
        if (data.Enemy_Rank.Equals(EnemySpawnRankType.Boss))
        {
            EndGame(false);
        }
        // todo: UI 완료되면 활성화
        /*chaseGuage += chaseIncreaseRate;
        if (chaseGuage >= chaseGuageMax)
        {
            EndGame(false);
        }*/
    }

    private async void Start()
    {
        Debug.Log("[BattleManager] Start called. Loading all resources.");
        loadUI.SetActive(true);
        await LoadAllResourcesAsync();
        StartGame();
    }

    public async Task LoadAllResourcesAsync()
    {
        var loadables = FindObjectsOfType<MonoBehaviour>().OfType<ILoadable>().Where(l => !l.IsLoaded).ToList();
        var tasks = loadables.Select(l => l.LoadAsync()).ToArray();

        LoadCompleteEventArg[] results = await Task.WhenAll(tasks);

        // 결과 처리 (성공/실패 여부)
        if (results.All(r => r.Success))
        {
            Debug.Log("모든 리소스 로드 완료");
        }
        else
        {
            foreach(var r in results.Where(r => !r.Success))
                Debug.LogError($"로드 실패: {r.ErrorMessage}");
            // 실패시 처리
        }
        
    }

    public void StartGame() // todo: 로딩 연동 필요
    {
        loadUI.SetActive(false);
        
        if(BackendStageGameData.stage == null)
        {
            // Debug.LogWarning("[BattleManager] BackendStageGameData.stage is null. Using default stage index 1.");
        }
        else
        {
            StageIndex = BackendStageGameData.stage.Currentstage;; // For testing purposes, remove later
        }
        
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
    void EndGame(bool isVictory)
    {
        isEnded = true;
        // Here you can handle the end of the game, such as showing a UI or transitioning to another scene
        Debug.Log(isVictory ? "Battle ended with victory!" : "Battle ended with defeat!");
        
        // Call the end battle event
        EndBattleEventArgs endEventArgs = new EndBattleEventArgs(isVictory); // Assuming victory for now
        BattleEventManager.Instance.CallEvent(endEventArgs);
        
        SessionManager.Instance.EndGame(isVictory);
    }
    
    // TODO: Implement logic to get monster by collider
    public static bool GetMonsterBy(Collider2D collider ,out Monster monster)
    {
        return collider.TryGetComponent(out monster);
    }
    
    // TODO: Implement logic to get all monsters in the battle
    public static IEnumerable<Monster> GetAllMonsters()
    {
        return null;
    }
    
    public static List<Monster> GetAllEnemyInRadius(Vector3 position, float radius)
    {
        var inRadius = Physics2D.OverlapCircleAll(position, radius, LayerMask.GetMask("Monster"));
        List<Monster> enemies = new List<Monster>();
        foreach (var collider in inRadius)
        {
            if (GetMonsterBy(collider, out Monster monster))
            {
                enemies.Add(monster);
            }
        }
        return enemies;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleManager : MonoSingleton<BattleManager>
{
    public int StageIndex = 0;
    [SerializeField] private Player player;
    
    [Header("전투 시간 세팅")]
    [SerializeField] private float battleDuration = 120f; // 2 minutes
    private float battleStartTime = 0f;
    
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
        if (args.Target.Equals(player))
        {
            Debug.Log("Player has died. Ending game.");
            EndGame(false);
        }
    }

    private void OnAliveMonster(AliveMonsterEventArgs obj)
    {
        chaseGuage += chaseIncreaseRate;
        if (chaseGuage >= chaseGuageMax)
        {
            EndGame(false);
        }
    }

    public void StartGame()
    {
        battleStartTime = Time.time;
        
        StartBattleEventArgs startEventArgs = new StartBattleEventArgs(StageIndex);
        
        BattleEventManager.Instance.CallEvent(startEventArgs);
    }

    private void Update()
    {
        if (isEnded)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
        
        float battleElapsedTime = Time.time - battleStartTime;
        if (battleElapsedTime >= battleDuration)
        {
            battleElapsedTime = Time.time - battleStartTime;

            // Check if the battle duration has been reached
            if (battleElapsedTime >= battleDuration)
            {
                EndGame(true);
                return;
            }
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
        EndBattleEventArgs endEventArgs = new EndBattleEventArgs(true); // Assuming victory for now
        BattleEventManager.Instance.CallEvent(endEventArgs);
    }
}

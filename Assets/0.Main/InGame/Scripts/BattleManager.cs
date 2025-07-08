using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleManager : MonoSingleton<BattleManager>
{
    public int StageIndex = 0;
    public void StartGame()
    {
        StartBattleEventArgs startEventArgs = new StartBattleEventArgs(StageIndex);
        
        BattleEventManager.Instance.CallEvent(startEventArgs);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    // Update is called once per frame
    void EndGame()
    {
        
    }
    
    // TODO: Implement logic to get monster by collider
    public static bool GetMonsterBy(Collider2D collider ,out Monster monster)
    {
        monster = null;
        return true;
    }
    
    // TODO: Implement logic to get all monsters in the battle
    public static IEnumerable<Monster> GetAllMonsters()
    {
        return null;
    }
    
    public static List<Monster> GetAllEnemyInRadius(Vector3 position, float radius)
    {
        var inRadius = Physics2D.OverlapCircleAll(position, radius, LayerMask.GetMask("Enemy"));
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

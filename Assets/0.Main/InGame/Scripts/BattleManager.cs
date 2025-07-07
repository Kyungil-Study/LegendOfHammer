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
}

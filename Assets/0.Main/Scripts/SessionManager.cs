using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : SingletonBase<SessionManager>
{
    public void StartGame() 
    { // todo: 로딩 연동 필요
        
        var stageIndex = BackendStageGameData.stage.Currentstage;; // For testing purposes, remove later
        SceneManager.LoadSceneAsync("Scene_Dungeon", LoadSceneMode.Single)!.completed += (operation) =>
        {
            Debug.Log("Loading scene completed.");
            BattleManager.Instance.StartGame(stageIndex);
        };
    }
    
    public void EndGame(bool success)
    {
        if (success)
        {
            Debug.Log("Game completed successfully.");
            BackendStageGameData.Instance.NextStage();
        }
        else
        {
            BackendStageGameData.Instance.ResetStage();
        }

        Debug.Log($"[SessionManager] Ending game");
        SceneManager.LoadSceneAsync("TestScene", LoadSceneMode.Single)!.completed += (operation) =>
        {
            Debug.Log("Returning to main menu.");
        };
    }
}

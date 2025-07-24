using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : SingletonBase<SessionManager>
{
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += SceneLoaded; 
    }

    private void SceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        if (scene.name == "Scene_Dungeon")
        {
            Debug.Log("Dungeon scene loaded.");
        }
    }


    public void GoToGameScene() 
    { // todo: 로딩 연동 필요

        Debug.Log($"[SessionManager] Starting game");
        
        SceneManager.LoadScene("Scene_Dungeon", LoadSceneMode.Single);
        //BackendStageGameData.Instance.PlayGame();

    }

    public void NextGame()
    {
        ES3Manager.Instance.NextStage();
        Debug.Log($"[SessionManager] Proceeding to next game stage");
        SceneManager.LoadScene("Scene_Dungeon", LoadSceneMode.Single);
    }
    
    public void EndGame(bool success)
    {
        if (success)
        {
            Debug.Log("Game completed successfully.");
            ES3Manager.Instance.NextStage();
        }
        else
        {
            ES3Manager.Instance.ResetCurrentStage();
        }
        SceneManager.LoadScene("Scene_Dungeon", LoadSceneMode.Single);
    }
}

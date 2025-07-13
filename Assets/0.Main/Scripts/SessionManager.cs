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
        if(scene.name == "Scene_Loading")
        {
            Debug.Log("Loading scene loaded.");
            LoadAllResourcesAsync();
        }
        else if (scene.name == "Scene_Dungeon")
        {
            Debug.Log("Dungeon scene loaded.");
            StartGame();
        }
    }

    public void GoToLoadScene()
    {
        Debug.Log($"[SessionManager] Going to load scene");
        
        // Load the loading scene
        SceneManager.LoadScene("Scene_Loading", LoadSceneMode.Single);
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
            GoToGameScene();
        }
        else
        {
            foreach(var r in results.Where(r => !r.Success))
                Debug.LogError($"로드 실패: {r.ErrorMessage}");
            // 실패시 처리
        }
        
    }

    void StartGame()
    {
        Debug.Log($"[SessionManager] Starting game");
        var stageIndex = BackendStageGameData.stage.Currentstage;; // For testing purposes, remove later
        BattleManager.Instance.StartGame(stageIndex);
    }

    public void GoToGameScene() 
    { // todo: 로딩 연동 필요

        Debug.Log($"[SessionManager] Starting game");
        var stageIndex = BackendStageGameData.stage.Currentstage;; // For testing purposes, remove later
        SceneManager.LoadScene("Scene_Dungeon", LoadSceneMode.Single);

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
        SceneManager.LoadScene("Scene_Loading", LoadSceneMode.Single);
    }
}

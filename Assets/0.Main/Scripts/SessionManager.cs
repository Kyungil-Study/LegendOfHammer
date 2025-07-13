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
    
    public void StartLoad()
    {
        Debug.Log($"[SessionManager] Starting game load");
        
        // Load all ILoadable components in the scene
        var monos = FindObjectsOfType<MonoBehaviour>();
        
        List<ILoadable> loadables = new List<ILoadable>();
        foreach (var mono in monos)
        {
            if(mono is ILoadable { IsLoaded: false } loadable)
            {
                loadables.Add(loadable);
            }   
        }
        
        int loadCount = loadables.Count;
        if (loadCount == 0)
        {
            Debug.Log("All loadables have been loaded.");
            gotoGame = true; // Set flag to start the game in the next frame
        }
        
        foreach (var loadable in loadables)
        {
            loadable.Load((args) =>
            {
                loadCount--;
                if (args.Success)
                {
                    Debug.Log($"{loadable.GetType().Name} loaded successfully.");
                }
                else
                {
                    Debug.LogError($"{loadable.GetType().Name} failed to load: {args.ErrorMessage}");
                }

                if (loadCount <= 0)
                {
                    Debug.Log("All loadables have been loaded.");
                    gotoGame = true; // Set flag to start the game in the next frame
                }
            });
        }
    }

    bool gotoGame = false;
    bool startGame = false;
    private void Update()
    {
        if (gotoGame)
        {
            gotoGame = false; // Reset the flag to prevent multiple calls
            GoToGameScene();
        }
    }

    void StartGame()
    {
        startGame = false; // Reset the flag to prevent multiple calls
        Debug.Log($"[SessionManager] Starting game");
        var stageIndex = BackendStageGameData.stage.Currentstage;; // For testing purposes, remove later
        BattleManager.Instance.StartGame(stageIndex);
    }

    public void GoToGameScene() 
    { // todo: 로딩 연동 필요

        gotoGame = false; // Reset the flag to prevent multiple calls
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

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : SingletonBase<SessionManager>
{
    [SerializeField] private TMP_InputField stageIndexInputField; // todo: For testing purposes, remove later
    int stageIndex = 1; // todo: For testing purposes, remove later
    public void StartGame()
    { // todo: 로딩 연동 필요
        
        stageIndex = int.Parse(stageIndexInputField.text); // For testing purposes, remove later
        SceneManager.LoadSceneAsync("Scene_Dungeon", LoadSceneMode.Single)!.completed += (operation) =>
        {
            Debug.Log("Loading scene completed.");
            BattleManager.Instance.StartGame(stageIndex);
        };
    }
    
    public void EndGame()
    {
        Debug.Log($"[SessionManager] Ending game");
        SceneManager.LoadSceneAsync("TestScene", LoadSceneMode.Single)!.completed += (operation) =>
        {
            Debug.Log("Returning to main menu.");
        };
    }
}

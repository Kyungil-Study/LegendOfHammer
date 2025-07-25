using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameLobbyPage : UIPage
{
    [SerializeField] private Button gameBeginButton;
    
    [SerializeField] private TMP_Text currentStageText;
    [SerializeField] private TMP_Text maxStageText;
    
    [SerializeField] private TMP_InputField userTryStageInputField;
    [SerializeField] private Button userTryStageButton;
    
    public override UIPageType UIPageType => UIPageType.LobbyPage;
    
    private IPageFlowManageable Owner;
    public override void Initialize(IPageFlowManageable owner)
    {
        Owner = owner ?? throw new System.ArgumentNullException(nameof(owner), "Owner cannot be null.");
        gameBeginButton.onClick.AddListener(OnGameBegin);
        userTryStageButton.onClick.AddListener(OnUserTryStage);
    }

    private void OnUserTryStage()
    {
        var stageData = ES3Manager.Instance.StageData;
        if (int.TryParse(userTryStageInputField.text, out int stageNumber))
        {
            ES3Manager.Instance.SetStage(stageNumber);
            currentStageText.text = stageNumber.ToString();
            maxStageText.text = stageData.MaxStage.ToString();
            Debug.Log($"User try stage set to: {stageNumber}");
        }
        else
        {
            Debug.LogWarning("Invalid stage number input. Please enter a valid integer.");
        }
    }

    private void OnGameBegin()
    {
        Debug.Log("Game Begin Button Clicked");
        // Notify the owner to start the game
        Owner.SwapPage( UIPageType.ClassAumgentSelection); // Assuming the next page is ClassAugmentSelection
        BattleManager.Instance.ReadyGame();
    }

    public override void Enter()
    {
        gameObject.SetActive(true);
        // Set current stage and max stage text
        var stageData = ES3Manager.Instance.StageData;
        currentStageText.text = stageData.CurrentStage.ToString();
        maxStageText.text = stageData.MaxStage.ToString();

        // Set user try stage input field to current stage
        userTryStageInputField.text = stageData.CurrentStage.ToString();
    }

    public override void Exit()
    {
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    
    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}

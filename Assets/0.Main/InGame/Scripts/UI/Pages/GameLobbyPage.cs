using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameLobbyPage : UIPage
{
    [Header("게임시작")]
    [SerializeField] private Button gameBeginButton;
    [SerializeField] private Button gameCloseButton;
    
    [Space(10),Header("유저 정보")]
    [SerializeField] private TMP_Text currentStageText;
    [SerializeField] private TMP_Text maxStageText;

    // todo: 이 부분은 출시때 제거할 것
    #region  Deprecate On Release 

    [Space(10),Header("유저 스테이지 시도")]
    [SerializeField] private TMP_InputField userTryStageInputField;
    [SerializeField] private Button userTryStageButton;
    
    [Space(10),Header("공용 증강 테스트 설정")]
    [SerializeField] private TMP_InputField commonAugmentIDInputField;
    [SerializeField] private Button commonAugmentAddButton;
    
    [Space(10),Header("클래스 증강 테스트 설정")]
    [SerializeField] private TMP_InputField classAugmentOptionIDInputField;
    [SerializeField] private TMP_InputField classAugmentLevelInputField;
    [SerializeField] private Button classAugmentSetButton;
    
    [SerializeField] private Button clearAugmentButton;

    #endregion
    
    public override UIPageType UIPageType => UIPageType.LobbyPage;
    
    protected override void Initialize(IPageFlowManageable owner)
    {
        gameBeginButton.onClick.AddListener(OnGameBegin);
        userTryStageButton.onClick.AddListener(OnUserTryStage);
        
        
        commonAugmentAddButton.onClick.AddListener(OnCommonAugmentAdd);
        classAugmentSetButton.onClick.AddListener(OnClassAugmentSet);
        clearAugmentButton.onClick.AddListener(OnClearAugment);
        
        gameCloseButton.onClick.AddListener(OnGameClose);
    }

    private void OnGameClose()
    {
        Debug.Log("Game Close Button Clicked");
        // Notify the owner to close the game
        Application.Quit();
    }

    private void OnClearAugment()
    {
        Debug.Log("Clearing all augments from inventory.");
        AugmentInventory.Instance.ClearInventory();
    }

    private void OnClassAugmentSet()
    {
        if(int.TryParse(classAugmentOptionIDInputField.text , out int classAugmentOptionID) == false)
        {
            Debug.LogWarning("Invalid Class Augment Option ID input. Please enter a valid integer.");
            return;
        }
        
        if(int.TryParse(classAugmentLevelInputField.text, out int classAugmentLevel) == false)
        {
            Debug.LogWarning("Invalid Class Augment Level input. Please enter a valid integer.");
            return;
        }

        var augment = ClassAugmentManager.Instance.GetAugmentWithOptionAndLevel(classAugmentOptionID, classAugmentLevel);
        if (augment == null)
        {
            Debug.LogWarning($"No Class Augment found with Option ID: {classAugmentOptionID} and Level: {classAugmentLevel}");
            return;
        }
        
        AugmentInventory.Instance.UpdateAugumetToInventory(augment);
    }

    private void OnCommonAugmentAdd()
    {
        if (int.TryParse(commonAugmentIDInputField.text, out int augmentID) == false)
        {
            Debug.LogWarning("Invalid Common Augment ID input. Please enter a valid integer.");
            return;
        }
        
        if(CommonAugmentManager.Instance.Records.TryGetValue(augmentID , out CommonAugment augment) == false)
        {
            Debug.LogWarning($"No Common Augment found with ID: {augmentID}");
            return;
        }
        
        AugmentInventory.Instance.UpdateAugumetToInventory(augment);
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
        if(AugmentInventory.Instance.IsFullClassAugment())
        {
            BattleUIController.Instance.SwapPage(UIPageType.CommonAugmentSelection);
        }
        else
        {
            BattleUIController.Instance.SwapPage( UIPageType.ClassAumgentSelection); // Assuming the next page is ClassAugmentSelection
        }
    }

    public override void Enter()
    {
        SoundManager.Instance.PauseRandomGameBgm();
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

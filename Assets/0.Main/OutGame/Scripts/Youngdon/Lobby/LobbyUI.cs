using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    [Header("User Info")]
    public TMP_Text userIdText;
    public TMP_Text nicknameText;

    [Header("Stage Info")]
    public TMP_Text currentStageText;
    public TMP_Text maxStageText;

    public TMP_InputField stageNameInput;
    public Button playButton;

    private void Awake()
    {
        playButton.onClick.AddListener(GoDungeonScene);
    }

    private void Start()
    {
        // User 정보 세팅
        userIdText.text     = $"UserID: {UserData.Instance.UserID}";
        nicknameText.text   = $"{UserData.Instance.Nickname}";
        
        // 시작 시 서버에서 스테이지 정보 가져오기
        BackendStageGameData.Instance.GetStage();
        if (BackendStageGameData.stage.Maxstage == 0)
        {
            BackendStageGameData.Instance.ResetStage();
        }
        RefreshStageUI();
    }

    private void Update()
    {
        // C 키 누르면 다음 스테이지로
        if (Input.GetKeyDown(KeyCode.C))
        {
            TestNextStage();
            Debug.Log("C pressed → NextStage()");
        }

        // U 키 누르면 서버에 업데이트
        if (Input.GetKeyDown(KeyCode.U))
        {
            TestUpdateDataToBackend();
            Debug.Log("U pressed → UpdateStage()");
        }

        // R 키 누르면 UI 새로 고침
        if (Input.GetKeyDown(KeyCode.R))
        {
            RefreshStageUI();
            Debug.Log("R pressed → RefreshStageUI()");
        }
    }

    public void RefreshStageUI()
    {
        var stage = BackendStageGameData.stage;
        currentStageText.text = $"{stage.Currentstage}";
        maxStageText.text     = $"{stage.Maxstage}";
    }

    public void GoDungeonScene()
    {
        BackendStageGameData.Instance.UpdateStage();
        SessionManager.Instance.GoToGameScene();
    }
    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(GoDungeonScene);
    }

    // 뒤끝 연동 디버그용 메서드
    public void TestNextStage()
    {
        BackendStageGameData.Instance.NextStage();
    }
    // 뒤끝 연동 디버그용 메서드
    public void TestUpdateDataToBackend()
    {
        BackendRank.Instance.RankInsert(BackendStageGameData.stage.Maxstage);
        BackendStageGameData.Instance.UpdateStage();
    }
    
}
using UnityEngine;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    [Header("User Info")]
    public TMP_Text userIdText;
    public TMP_Text nicknameText;

    [Header("Stage Info")]
    public TMP_Text currentStageText;
    public TMP_Text maxStageText;

    private void Start()
    {
        // User 정보 세팅
        userIdText.text     = $"UserID: {UserData.Instance.UserID}";
        nicknameText.text   = $"Nickname: {UserData.Instance.Nickname}";
        
        // 시작 시 서버에서 스테이지 정보 가져오기
        BackendStageGameData.Instance.GetStage();
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
        currentStageText.text = $"CurStage: {stage.Currentstage}";
        maxStageText.text     = $"MaxStage: {stage.Maxstage}";
    }

    public void TestNextStage()
    {
        BackendStageGameData.Instance.NextStage();
    }

    public void TestUpdateDataToBackend()
    {
        BackendStageGameData.Instance.UpdateStage();
    }
}
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
        userIdText.text = $"UserID: {UserData.Instance.UserID}";
        nicknameText.text = $"Nickname: {UserData.Instance.Nickname}";

      //  // Stage 정보 세팅
      //  if (StageData.Instance != null)
      //  {
      //      currentStageText.text = $"CurStage: {StageData.Instance.CurrentStage}";
      //      maxStageText.text = $"MaxStage: {StageData.Instance.MaxStage}";
      //  }
      //  else
      //  {
      //      currentStageText.text = "CurStage: 1";
      //      maxStageText.text = "MaxStage: 1";
      //  }
    }
}
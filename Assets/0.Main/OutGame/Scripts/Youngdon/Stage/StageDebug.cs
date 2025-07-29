using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageDebug : MonoBehaviour
{
    [Header("디버그 UI")]
    [SerializeField] private TMP_InputField stageInputField;
    [SerializeField] private Button stageSetButton;

    private void Awake()
    {
        // Button 클릭 시 SetStageDebug 호출
        stageSetButton.onClick.AddListener(SetStageDebug);
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지
        stageSetButton.onClick.RemoveListener(SetStageDebug);
    }

    private void SetStageDebug()
    {
        string input = stageInputField.text.Trim();

        if (int.TryParse(input, out int newStage))
        {
            BackendStageGameData.stage.Currentstage = newStage;

            // currentStage가 maxStage보다 크면 maxStage도 같이 수정
            if (newStage > BackendStageGameData.stage.Maxstage)
            {
                BackendStageGameData.stage.Maxstage = newStage;
            }

            // 뒤끝에 반영
            BackendStageGameData.Instance.UpdateStage();
            Debug.Log($"[StageDebug] currentStage를 {newStage}로 변경하고 저장했습니다.");
        }
        else
        {
            Debug.LogWarning("[StageDebug] 입력값이 숫자가 아닙니다.");
        }
    }
}
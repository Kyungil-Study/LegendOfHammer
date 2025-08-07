using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClearPage : UIPage
{
    [SerializeField] Button clearNextButton;
    [SerializeField] Button clearExitButton;
    
    [SerializeField] TMP_Text attemptCountText;
    [SerializeField] TMP_Text clearStageText;
    [SerializeField] TMP_Text clearScoreText;
    
    public override UIPageType UIPageType => UIPageType.ClearPage;
    protected override void Initialize(IPageFlowManageable owner)
    {
        clearNextButton.onClick.AddListener(NextGame);
        clearExitButton.onClick.AddListener(ExitGame);
    }

    private void ExitGame()
    {
        SessionManager.Instance.EndGame(true);
    }

    private void NextGame()
    {
        SessionManager.Instance.NextGame();
    }

    public override void Enter()
    {
        AugmentInventory.Instance.SaveData();
        BattleEventManager.CallEvent(new PauseBattleEventArgs(true));
        var stageData = ES3Manager.Instance.StageData;
        clearStageText.text = stageData.CurrentStage.ToString();
        clearScoreText.text = BattleManager.Instance.Score.ToString();
        attemptCountText.text = stageData.StageAttemptCount.ToString();
        gameObject.SetActive(true);
    }

    public override void Exit()
    {
        BattleEventManager.CallEvent(new PauseBattleEventArgs(false));  
        gameObject.SetActive(false);
    }
}
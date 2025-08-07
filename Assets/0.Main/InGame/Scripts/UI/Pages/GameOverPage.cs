using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPage : UIPage
{
    [SerializeField] Button gameOverReviveButton;
    [SerializeField] Button gameOverExitButton;

    [SerializeField] TMP_Text attemptCountText;
    [SerializeField] TMP_Text stageText;
    [SerializeField] TMP_Text scoreText;
    public override UIPageType UIPageType => UIPageType.GameOverPage;
    protected override void Initialize(IPageFlowManageable owner)
    {
        gameOverExitButton.onClick.AddListener(ExitGame);
        gameOverReviveButton.onClick.AddListener(ReviveGame);
    }

    private void ReviveGame()
    {
        if (BattleManager.Instance.IsBossAlived)
        {
            Owner.SwapPage(UIPageType.ClearPage);
        }
        else
        {
            BattleEventManager.CallEvent(new PauseBattleEventArgs(false));
            BattleManager.Instance.Revive();
            Owner.SwapPage(UIPageType.BattlePage);
        }
    }

    private void ExitGame()
    {
        BattleEventManager.CallEvent(new PauseBattleEventArgs(false));
        SessionManager.Instance.EndGame(false);
    }

    public override void Enter()
    {
        gameObject.SetActive(true);
        var stageData = ES3Manager.Instance.StageData;
        stageText.text = stageData.CurrentStage.ToString();
        scoreText.text = BattleManager.Instance.Score.ToString();
        attemptCountText.text = stageData.StageAttemptCount.ToString();
        BattleEventManager.CallEvent(new PauseBattleEventArgs(true));
    }

    public override void Exit()
    {
        gameObject.SetActive(false);
        BattleEventManager.CallEvent(new PauseBattleEventArgs(false));

    }
}
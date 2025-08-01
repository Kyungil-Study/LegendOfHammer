using UnityEngine;
using UnityEngine.UI;

public class GameOverPage : UIPage
{
    [SerializeField] Button gameOverReviveButton;
    [SerializeField] Button gameOverExitButton;

    public override UIPageType UIPageType => UIPageType.GameOverPage;
    public override void Initialize(IPageFlowManageable owner)
    {
        gameOverExitButton.onClick.AddListener(ExitGame);
        gameOverReviveButton.onClick.AddListener(ReviveGame);
    }

    private void ReviveGame()
    {
        BattleManager.Instance.Revive();
    }

    private void ExitGame()
    {
        SessionManager.Instance.EndGame(false);
    }

    public override void Enter()
    {
        gameOverReviveButton.gameObject.SetActive(Squad.Instance.IsDead);
        gameObject.SetActive(true);
    }

    public override void Exit()
    {
        gameObject.SetActive(false);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class ClearPage : UIPage
{
    [SerializeField] Button clearNextButton;
    [SerializeField] Button clearExitButton;
    
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
        gameObject.SetActive(true);
    }

    public override void Exit()
    {
        BattleEventManager.CallEvent(new PauseBattleEventArgs(false));  
        gameObject.SetActive(false);
    }
}
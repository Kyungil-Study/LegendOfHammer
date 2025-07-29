using UnityEngine;
using UnityEngine.UI;

public class ClearPage : UIPage
{
    [SerializeField] Button clearNextButton;
    [SerializeField] Button clearExitButton;
    
    public override UIPageType UIPageType => UIPageType.ClearPage;
    public override void Initialize(IPageFlowManageable owner)
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
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public override void Exit()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
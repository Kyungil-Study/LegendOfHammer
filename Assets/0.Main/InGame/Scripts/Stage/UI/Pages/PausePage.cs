using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePage : UIPage
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    private IPageFlowManageable owner;
    
    public override UIPageType UIPageType => UIPageType.PausePage;
    
    public override void Initialize(IPageFlowManageable owner)
    {
        this.owner = owner;
        resumeButton.onClick.AddListener(()=>{owner.SwapPage(UIPageType.BattlePage);});
        quitButton.onClick.AddListener(OnQuitGame);
    }

    private void OnQuitGame()
    {
        SessionManager.Instance.QuitGame();
    }

    public override void Enter()
    {
        // PausePage에 진입할 때 필요한 초기화 작업을 수행합니다.
        Time.timeScale = 0;
    }

    public override void Exit()
    {
        // PausePage에서 나갈 때 필요한 정리 작업을 수행합니다.
        Time.timeScale = 1;
    }
}

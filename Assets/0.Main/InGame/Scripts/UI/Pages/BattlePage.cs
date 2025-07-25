using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePage : UIPage
{
    [Header("스테이지")]
    [LabelText("스테이지 번호"), SerializeField] private TMP_Text stageNumberText;
    [LabelText("최대 스테이지 번호"), SerializeField] private TMP_Text maxStageNumberText;

    [Header("점수")]
    [LabelText("현재 점수"), SerializeField] private TMP_Text currentScoreText;
    [LabelText("최대 점수"), SerializeField] private TMP_Text maxScoreText;
    
    [Header("추격")]
    [LabelText("추격 게이지"), SerializeField] private Slider chasingSlider;
    
    private Dictionary<EnemyID ,int> scoreMap = new Dictionary<EnemyID, int>();
    private int currentScore = 0;
    private int maxScore = 0;
    
    public override UIPageType UIPageType => UIPageType.BattlePage;
    private IPageFlowManageable owner;
    
    public override void Initialize(IPageFlowManageable owner)
    {
        this.owner = owner;
        
        currentScoreText.text = "0";
        maxScoreText.text = "0";
        
        var callbacks = BattleEventManager.Instance.Callbacks;
        callbacks.OnDeath += OnDeath; 
        callbacks.OnEndBattle += OnEndBattle;
        
        BattleManager.Instance.ChaseGuage.Events.OnValueChanged += OnChaseGuageChanged;
        chasingSlider.maxValue = BattleManager.Instance.ChaseGuage.Max;
    }

    private void OnChaseGuageChanged(float arg1, float arg2)
    {
        chasingSlider.value = arg1;
    }


    private void OnEndBattle(EndBattleEventArgs args)
    {
        if (args.IsVictory)
        {
            if (args.IsBoosDead)
            {
                owner.SwapPage(UIPageType.CommonAugmentSelection);
            }
            else
            {
                owner.SwapPage(UIPageType.ClearPage);
            }
        }
        else
        {
            owner.SwapPage(UIPageType.GameOverPage);
        }
        
    }

    private void OnDeath(DeathEventArgs args)
    {
        if (args.Target is Monster monster)
        {
            var records = EnemyDataManager.Instance.Records;
            var score = records[monster.EnemyID].Chasing_Increase;
            currentScore += score;
            maxScore = Mathf.Max(maxScore, currentScore);
            
            currentScoreText.text = currentScore.ToString();
            maxScoreText.text = maxScore.ToString();
        }
        
    }

    public override void Enter()
    {
        var stageIndex = BattleManager.Instance.StageIndex;
        var maxStageIndex = BattleManager.Instance.MaxStageNumber;
        stageNumberText.text = stageIndex.ToString();
        maxStageNumberText.text = maxStageIndex.ToString();

        gameObject.SetActive(true);
    }

    public override void Exit()
    {
        gameObject.SetActive(false);
    }
}
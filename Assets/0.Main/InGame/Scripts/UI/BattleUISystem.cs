using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoSingleton<BattleUIController>
{
    [Header("스테이지")]
    [LabelText("스테이지 번호"), SerializeField] private TMP_Text stageNumberText;
    [LabelText("최대 스테이지 번호"), SerializeField] private TMP_Text maxStageNumberText;

    [Header("점수")]
    [LabelText("현재 점수"), SerializeField] private TMP_Text currentScoreText;
    [LabelText("최대 점수"), SerializeField] private TMP_Text maxScoreText;
    
    [Header("증강 메뉴")]
    [LabelText("증강 메뉴 오브젝트"), SerializeField] private Button augmentMenuObject;
    
    [Header("게임 결과")]
    [SerializeField] RectTransform clearMenuObject;
    [SerializeField] Button clearNextButton;
    [SerializeField] Button clearExitButton;
    
    [SerializeField] RectTransform gameOverMenuObject;
    [SerializeField] Button gameOverReviveButton;
    [SerializeField] Button gameOverExitButton;
    
    
    private Dictionary<EnemyID ,int> scoreMap = new Dictionary<EnemyID, int>();
    private int currentScore = 0;
    private int maxScore = 0;
    bool isVictory;

    // Start is called before the first frame update
    void Start()
    {
        var callbacks = BattleEventManager.Instance.Callbacks;
        callbacks.OnReadyBattle += OnReadBattle;
        callbacks.OnDeath += OnDeath; 
        clearNextButton.onClick.AddListener(NextGame);
        
        clearExitButton.onClick.AddListener(ExitGame);
        gameOverExitButton.onClick.AddListener(ExitGame);

        callbacks.OnEndBattle += OnEndBattle;

    }

    public void OnClear()
    {
        isVictory = true;
        clearMenuObject.gameObject.SetActive(true);
    }
    
    public void OnGameOver()
    {
        isVictory = false;
        gameOverMenuObject.gameObject.SetActive(true);
    }

    private void OnEndBattle(EndBattleEventArgs args)
    {
        isVictory = args.IsVictory;
        if (isVictory)
        {
            AugmentGotchaSystem.Instance.OnOpenCommonAugment();
        }
        else
        {
            gameOverMenuObject.gameObject.SetActive(true);
        }
    }

    private void OnReadBattle(ReadyBattleEventArgs args)
    {
        stageNumberText.text = args.StageIndex.ToString();
        maxStageNumberText.text = args.MaxStageIndex.ToString();
        currentScoreText.text = "0";
        maxScoreText.text = "0";
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
    
    private void NextGame()
    {
        SessionManager.Instance.NextGame();
    }

    void ExitGame()
    {
        SessionManager.Instance.EndGame(isVictory);
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultUIController : MonoBehaviour
{
    [SerializeField] RectTransform resultPanel;
    [SerializeField] TMP_Text resultText;
    [SerializeField] Button nextButton;
    [SerializeField] Button exitButton;
    
    bool isVictory;
    
    // Start is called before the first frame update
    void Start()
    {
        exitButton.onClick.AddListener(ExitGame);
        nextButton.onClick.AddListener(NextGame);

        var callbacks = BattleEventManager.Instance.Callbacks;
        callbacks.OnEndBattle += (args) =>
        {
            isVictory = args.IsVictory;
            resultText.text = isVictory ? "Victory!" : "Defeat!";
            
            resultPanel.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(isVictory);
        };
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

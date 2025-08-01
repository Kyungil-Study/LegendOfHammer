using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class PausePage : UIPage
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    
    [SerializeField] private AugmentInventorySlot[] commonAugmentSlots;
    [SerializeField] private AugmentInventorySlot[] warriorAugmentSlots;
    [SerializeField] private AugmentInventorySlot[] wizardAugmentSlots;
    [SerializeField] private AugmentInventorySlot[] archerAugmentSlots;
    
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
        BattleEventManager.CallEvent(new PauseBattleEventArgs(true));
        
        var inventory = AugmentInventory.Instance;
        for (int i = 0;i < commonAugmentSlots.Length; i++)
        {
            if (i < inventory.CommonAugments.Count)
            {
                commonAugmentSlots[i].gameObject.SetActive(true);
                commonAugmentSlots[i].Initialize(inventory.CommonAugments[i]);
            }
            else
            {
                commonAugmentSlots[i].gameObject.SetActive(false);
            }
        }
        
        for (int i = 0;i < warriorAugmentSlots.Length; i++)
        {
            if (i < inventory.WarriorAugments.Count)
            {
                warriorAugmentSlots[i].gameObject.SetActive(true);
                warriorAugmentSlots[i].Initialize(inventory.WarriorAugments[i]);
            }
            else
            {
                warriorAugmentSlots[i].gameObject.SetActive(false);
            }
        }
        
        for (int i = 0;i < wizardAugmentSlots.Length; i++)
        {
            if (i < inventory.WizardAugments.Count)
            {
                wizardAugmentSlots[i].gameObject.SetActive(true);
                wizardAugmentSlots[i].Initialize(inventory.WizardAugments[i]);
            }
            else
            {
                wizardAugmentSlots[i].gameObject.SetActive(false);
            }
        }
        
        for (int i = 0;i < archerAugmentSlots.Length; i++)
        {
            if (i < inventory.ArcherAugments.Count)
            {
                archerAugmentSlots[i].gameObject.SetActive(true);
                archerAugmentSlots[i].Initialize(inventory.ArcherAugments[i]);
            }
            else
            {
                archerAugmentSlots[i].gameObject.SetActive(false);
            }
        }
        
        
        
    }

    public override void Exit()
    {
        BattleEventManager.CallEvent(new PauseBattleEventArgs(false));
    }
}

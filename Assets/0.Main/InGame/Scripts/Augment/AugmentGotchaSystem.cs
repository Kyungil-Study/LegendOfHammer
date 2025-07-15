using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AugmentGotchaSystem : MonoBehaviour
{
    [SerializeField] private RectTransform augmentPanel;
    [SerializeField] private AugmentSlot[] augmentSlots;
    [SerializeField] private Button warriorSlot;
    [SerializeField] private Button wizardSlot;
    [SerializeField] private Button archerSlot;
    
    [SerializeField] private Button rerollAugmentButton;
    
    // Start is called before the first frame update
    void Start()
    {
        BattleEventManager.Instance.Callbacks.OnReadyBattle += OnReadyBattle;
        BattleEventManager.Instance.Callbacks.OnSelectAugment += OnSelectAugment;
    }

    private void OnSelectAugment(SelectAugmentEventArgs args)
    {
        augmentPanel.gameObject.SetActive(false);
        BattleManager.Instance.StartGame();
    }

    public struct ProbabilityRecord<T>
    {
        public T ID;
        public int minProbability; // 확률
        public int maxProbability; // 확률
        
        public ProbabilityRecord(T id, int min, int max)
        {
            ID = id;
            minProbability = min;
            maxProbability = max;
        }
        
        public bool IsInRange(int value)
        {
            return value >= minProbability && value <= maxProbability;
        }
        
    }

    List<ProbabilityRecord<AugmentRarity>> rarityRecords = new List<ProbabilityRecord<AugmentRarity>>();
    private void OnReadyBattle(ReadyBattleEventArgs args)
    {
        augmentPanel.gameObject.SetActive(true);
        GotchaCommonAugment();
    }
    
    private void GotchaClassAugment()
    {
        var groups = ClassAugmentManager.Instance.AugmentOptionGroups;
        
        var groupIndex0 = Random.Range(0, groups.Count);
        var groupIndex1 = Random.Range(0, groups.Count);
        AugmentOptionGroup group0 = groups[groupIndex0];
        AugmentOptionGroup group1 = groups[groupIndex1];
    }
    
    private void GotchaCommonAugment()
    {
        // Initialize augment slots
        // 등급 가챠
        var probability = AugmentProbability.Instance.rarityRecords.Values.ToList();
        int totalProbability = 0;
        foreach (var item in probability)
        {
            int probabilityInteger = (int)(item.Probability * 100f); // Convert to percentage
            
            rarityRecords.Add(new ProbabilityRecord<AugmentRarity>
            {
                ID = item.Rarity,
                minProbability = totalProbability,
                maxProbability = totalProbability + probabilityInteger
            });
            totalProbability += probabilityInteger;
        }
        
        int randomValue = Random.Range(0, totalProbability);
        var rarityRecord = rarityRecords.First( r => r.IsInRange(randomValue));
        
        // 옵션 가챠
        
        var commonProbability = AugmentProbability.Instance.commonOptionRecords.Values.ToList();
        List<ProbabilityRecord<int>> commonOptionRecords = new List<ProbabilityRecord<int>>();
        totalProbability = 0;
        
        foreach (var item in commonProbability)
        {
            int probabilityInteger = (int)(item.Probability * 100f); // Convert to percentage
            
            commonOptionRecords.Add(new ProbabilityRecord<int>
            {
                ID = item.OptionID,
                minProbability = totalProbability,
                maxProbability = totalProbability + probabilityInteger
            });
            totalProbability += probabilityInteger;
        }
        randomValue = Random.Range(0, totalProbability);
        var optionRecord = commonOptionRecords.First( r => r.IsInRange(randomValue));
        randomValue = Random.Range(0, totalProbability);
        var optionRecord2 = commonOptionRecords.First( r => r.IsInRange(randomValue));
        
            
        
        var slot1 = CommonAugmentManager.Instance.GetAugmentFiltered(rarityRecord.ID, optionRecord.ID);
        var slot2 = CommonAugmentManager.Instance.GetAugmentFiltered(rarityRecord.ID, optionRecord2.ID);
        
        augmentSlots[0].SetAugment(slot1);
        augmentSlots[1].SetAugment(slot2);
    }

    
}

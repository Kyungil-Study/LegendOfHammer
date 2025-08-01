
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CommonAugmentGotchaSystem : UIPage
{
    [Header("Common Augment")]
    [SerializeField] private AugmentSlot[] commonAugmentSlots;
    [SerializeField] private Button commonRerollAugmentButton;

    private AugmentRarity minRarity = AugmentRarity.None;

    public override UIPageType UIPageType => UIPageType.CommonAugmentSelection;

    private IPageFlowManageable Owner;
    public override void Initialize(IPageFlowManageable owner)
    {
        Owner = owner ?? throw new System.ArgumentNullException(nameof(owner), "Owner cannot be null.");
        
        commonRerollAugmentButton.onClick.AddListener(() =>
        {
            Debug.Log("Reroll common augment button clicked.");
            // Handle reroll logic here
            GotchaCommonAugment();
        });
    }

    public override void Enter()
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
        GotchaCommonAugment();
    }

    public override void Exit()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
    
    private void GotchaCommonAugment()
    {
        Debug.Log("Gotcha common augment.");
        List<ProbabilityRecord<AugmentRarity>> rarityRecords = new List<ProbabilityRecord<AugmentRarity>>();
        // Initialize augment slots
        // 등급 가챠
        var probability = AugmentProbability.Instance.rarityRecords.Values.ToList();
        int totalProbability = 0;
        foreach (var item in probability)
        {
            //Debug.Log($"Rarity: {item.Rarity}, Probability: {item.Probability}");
            if (item.Rarity < minRarity)
            {
                //Debug.Log($"Skipping rarity {item.Rarity} as it is lower than the minimum rarity {minRarity}");
                continue; // Skip if the rarity is lower than the minimum rarity
            }
            
            int probabilityInteger = (int)(item.Probability * 1000f); // Convert to percentage
            var record = new ProbabilityRecord<AugmentRarity>
            {
                ID = item.Rarity,
                minProbability = totalProbability,
                maxProbability = totalProbability + probabilityInteger
            };
            rarityRecords.Add(record);
            Debug.Log($"Rarity ID: {record.ID}, Probability: {item.Probability}, Range: {record.minProbability} - {record.maxProbability}");
            totalProbability += probabilityInteger;
        }
        //Debug.Log($"Filtered RarityRecords : {rarityRecords.Count}");

        var rarityIndex = Random.Range(0, totalProbability);
        Debug.Log($"Selected rarity Index {rarityIndex}");
        var rarityRecord = rarityRecords.FirstOrDefault( r => r.IsInRange(rarityIndex));
        if (rarityRecord.ID == AugmentRarity.None)
        {
            Debug.LogError($"No valid rarity found. Please check the rarity records. {rarityRecord.ID}");
            return;
        }
        minRarity = rarityRecord.ID;
        //Debug.Log($"Selected rarity {rarityIndex}: {rarityRecord.ID} with probability range {rarityRecord.minProbability} - {rarityRecord.maxProbability}");

        
        // 옵션 가챠
        var commonProbability = AugmentProbability.Instance.commonOptionRecords.Values.ToList();
        List<ProbabilityRecord<int>> commonOptionRecords = new List<ProbabilityRecord<int>>();
        totalProbability = 0;
        
        foreach (var item in commonProbability)
        {
            int probabilityInteger = (int)(item.Probability * 1000f); // Convert to percentage
            var optionItem = new ProbabilityRecord<int>
            {
                ID = item.OptionID,
                minProbability = totalProbability,
                maxProbability = totalProbability + probabilityInteger
            };
            //Debug.Log( $"Option ID: {optionItem.ID}, Probability: {item.Probability}, Range: {optionItem.minProbability} - {optionItem.maxProbability}");
            commonOptionRecords.Add(optionItem);
            totalProbability += probabilityInteger;
        }
        var optionIndex = Random.Range(0, totalProbability);
        
        var optionRecord = commonOptionRecords.FirstOrDefault(r => r.IsInRange(optionIndex));
        commonOptionRecords.Remove(optionRecord);
        Debug.Log($"Selected option {optionIndex}: {optionRecord.ID} with probability range {optionRecord.minProbability} - {optionRecord.maxProbability}");
        
        do // 중복 예방 처리
        {
            optionIndex = Random.Range(0, totalProbability);
        } while ((commonOptionRecords.Exists(r => r.IsInRange(optionIndex)) == false));
        
        var optionRecord2 = commonOptionRecords.FirstOrDefault( r => r.IsInRange(optionIndex));
        Debug.Log($"Selected option2 {optionIndex} : {optionRecord2.ID} with probability range {optionRecord2.minProbability} - {optionRecord2.maxProbability}");
        
        var slot1 = CommonAugmentManager.Instance.GetAugmentFiltered(rarityRecord.ID, optionRecord.ID);
        if (slot1 == null)
        {
            Debug.LogError($"Failed to get augment for rarity {rarityRecord.ID} and option {optionRecord.ID}");
            return;
        }
        var slot2 = CommonAugmentManager.Instance.GetAugmentFiltered(rarityRecord.ID, optionRecord2.ID);
        if (slot2 == null)
        {
            Debug.LogError($"Failed to get augment for rarity {rarityRecord.ID} and option {optionRecord2.ID}");
            return;
        }
        commonAugmentSlots[0].SetAugment(slot1,OnSelectAugment);
        commonAugmentSlots[1].SetAugment(slot2,OnSelectAugment);
    }

    private void OnSelectAugment(Augment augment)
    {
        BattleEventManager.CallEvent(new SelectAugmentEventArgs(augment));
        if (BattleManager.Instance.IsEnded)
        {
            Owner.SwapPage(UIPageType.ClearPage);
        }
        else
        {
            Owner.SwapPage(UIPageType.BattlePage);
        }
    }
}
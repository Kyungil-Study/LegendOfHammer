
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CommonAugmentGotchaSystem : UIPage
{
    [Header("Common Augment")]
    [SerializeField] private AugmentSlot[] commonAugmentSlots;
    [SerializeField] private Button commonRerollAugmentButton;
    [SerializeField] private Button commonRerollEnterButton;
    [SerializeField] PageCinematic cinematicPanel;
    
    [SerializeField] private int rerollCost = 1000; // 현재 리롤 횟수
    private AugmentRarity curRarity = AugmentRarity.None;

    public override UIPageType UIPageType => UIPageType.CommonAugmentSelection;

    protected override void Initialize(IPageFlowManageable owner)
    {
        commonRerollAugmentButton.onClick.AddListener(RerollAugment);
    }

    private void RerollAugment()
    {
        Debug.Log("Reroll common augment button clicked.");
        // Handle reroll logic here
        GotchaCommonAugment();
        cinematicPanel.gameObject.SetActive(true);
        
        rerollCost -= 1;
        if (rerollCost <= 0)
        {
            commonRerollEnterButton.gameObject.SetActive(false);
        }
    }

    public override void Enter()
    {
        BattleEventManager.CallEvent(new PauseBattleEventArgs(true));
        gameObject.SetActive(true);
        GotchaCommonAugment();
    }

    public override void Exit()
    {
        BattleEventManager.CallEvent(new PauseBattleEventArgs(false));
        gameObject.SetActive(false);
        if (BattleManager.Instance.IsEnded == false)
        {
            BattleManager.Instance.StartGame();
        }
    }

    private AugmentRarity GotchaRarity()
    {
        Debug.Log("Gotcha common augment rarity.");
        List<ProbabilityRecord<AugmentRarity>> rarityRecords = new List<ProbabilityRecord<AugmentRarity>>();
        // Initialize augment slots
        // 등급 가챠
        var probability = AugmentProbability.Instance.rarityRecords.Values.ToList();
        int totalProbability = 0;
        foreach (var item in probability)
        {
            //Debug.Log($"Rarity: {item.Rarity}, Probability: {item.Probability}");
            if (item.Rarity < curRarity)
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
            return AugmentRarity.None;
        }
        curRarity = rarityRecord.ID;

        return rarityRecord.ID;
        //Debug.Log($"Selected rarity {rarityIndex}: {rarityRecord.ID} with probability range {rarityRecord.minProbability} - {rarityRecord.maxProbability}");
    }

    private bool TryGotchaOption(out int totalProbability,out List<ProbabilityRecord<int>> commonOptionRecords)
    {
        // 옵션 가챠
        var commonProbability = AugmentProbability.Instance.commonOptionRecords.Values.ToList();
        commonOptionRecords = new List<ProbabilityRecord<int>>();
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
        
        if(commonOptionRecords.Count == 0)
        {
            Debug.Log("No valid class augments available. Please check the augment records.");
            return false; // No valid augments to select
        }

        return true;
    }
    
    private void GotchaCommonAugment()
    {
        Debug.Log("Gotcha common augment.");
        
        GotchaRarity();
        
        if(TryGotchaOption(out int totalProbability, out List<ProbabilityRecord<int>> commonOptionRecords) == false)
        {
            Debug.LogError("Failed to get common augment options. Please check the augment records.");
            return;
        }
        
        Debug.Log($"Total probability: {totalProbability}");
        for(int i= 0 ; i < commonAugmentSlots.Length; i++)
        {
            GotchaSlot(i, totalProbability, commonOptionRecords);
        }
    }
    
    private void GotchaSlot(int slotIndex, int totalProbability, List<ProbabilityRecord<int>> probabilities)
    {
        if (probabilities.Count == 0) // 더이상 뽑을 증강이 없다.
        {
            commonAugmentSlots[slotIndex].gameObject.SetActive(false);
            Debug.Log("No probabilities available for the selected slot. Please check the augment records.");
            return;
        }
        
        int randomIndex = UnityEngine.Random.Range(0, totalProbability);
        do
        {
            randomIndex = UnityEngine.Random.Range(0, totalProbability);
        } while (probabilities.Any( r => r.IsInRange(randomIndex)) == false);
        
        var selectedID = probabilities.FirstOrDefault(r => r.IsInRange(randomIndex));
        if (selectedID.ID == 0)
        {
            Debug.LogError("No valid augment found. Please check the augment records.");
            return;
        }
        probabilities.Remove(selectedID); // 중복 방지를 위해 제거
        Debug.Log($"Selected Slot{slotIndex} augment IDs: {selectedID.ID}");
        commonAugmentSlots[slotIndex].gameObject.SetActive(true);
        commonAugmentSlots[slotIndex].SetAugment(CommonAugmentManager.Instance.GetAugmentFiltered(curRarity, selectedID.ID), OnSelectAugment);
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
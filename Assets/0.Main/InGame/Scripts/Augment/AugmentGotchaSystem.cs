using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AugmentGotchaSystem : MonoSingleton<AugmentGotchaSystem>
{
    [SerializeField] private RectTransform augmentPanel;
    [SerializeField] private AugmentSlot[] augmentSlots;
    [SerializeField] private Button warriorSlot;
    [SerializeField] private Button wizardSlot;
    [SerializeField] private Button archerSlot;
    
    [SerializeField] private Button rerollAugmentButton;
    
    AugmentType rerollAugmentType = AugmentType.Common;
    // Start is called before the first frame update
    void Start()
    {
        BattleEventManager.Instance.Callbacks.OnReadyBattle += OnReadyBattle;
        
        warriorSlot.onClick.AddListener(() => {
            Debug.Log("Warrior slot selected.");
            rerollAugmentType = AugmentType.Warrior;
            // Handle warrior slot selection logic here
        });
        wizardSlot.onClick.AddListener(() => {
            Debug.Log("Wizard slot selected.");
            rerollAugmentType = AugmentType.Wizard;
            // Handle wizard slot selection logic here
        });
        archerSlot.onClick.AddListener(() => {
            Debug.Log("Archer slot selected.");
            rerollAugmentType = AugmentType.Archer;
            // Handle archer slot selection logic here
        });
        
        rerollAugmentButton.onClick.AddListener(() => {
            Debug.Log("Reroll augment button clicked.");
            // Handle reroll logic here
            RerollGotchaAugment();
        });
    }

    private void RerollGotchaAugment()
    {
        switch (rerollAugmentType)
        {
            case AugmentType.Warrior:
            case AugmentType.Archer:
            case AugmentType.Wizard:
                RerollClassAugment();
                break;
            case AugmentType.Common:
                GotchaCommonAugment();
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    public void OnSelectAugment(Augment augment)
    {
        augmentPanel.gameObject.SetActive(false);
        BattleEventManager.Instance.CallEvent(new SelectAugmentEventArgs(augment));
        BattleManager.Instance.StartGame();
    }

    public struct ProbabilityRecord<T>
    {
        public T ID;
        public int minProbability; // 확률
        public int maxProbability;
        
        public ProbabilityRecord(T id, int min, int max)
        {
            ID = id;
            minProbability = min;
            maxProbability = max;
        }
        
        public bool IsInRange(int value)
        {
            // Check if the value is within the range of minProbability and maxProbability
            return value >= minProbability && value <= maxProbability;
        }
        
    }

    private void OnReadyBattle(ReadyBattleEventArgs args)
    {
        augmentPanel.gameObject.SetActive(true);
        GotchaClassAugment( ClassAugmentManager.Instance.GetAllOption());
        //GotchaCommonAugment();
    }
    
    private void GotchaClassAugment([NotNull] IReadOnlyList<int> options)
    {
        var inventory = AugmentInventory.Instance;
        Debug.Log("Gotcha class augment first.");
        // Initialize class augment slots with random augments

        var augmentGroupByOption = ClassAugmentManager.Instance.AugmentGroupByOption;
        var inventoryAugments = inventory.GetAllClassAugments();

        var probabilitiesRecord = AugmentProbability.Instance.classRecords;
        
        int totalProbability = 0;
        List<ProbabilityRecord<int>> probabilities = new List<ProbabilityRecord<int>>();
        foreach (var option in options)
        {
            int augmentID = 0;
            int probabilityInteger = 0; // 확률을 정수로 변환

            // 인베토리 조회 해당 옵션이 있다면 다음 증강 획득 가능한지 확인
            var includeInventory = inventoryAugments.Any(a => a.OptionID == option);
            if (includeInventory)
            {
                var data = inventoryAugments.First(a => a.OptionID == option );
                if (data.IsMaxLevel())
                {
                    continue; // 이미 최대 레벨인 경우 스킵
                }
                
                // 다음 레벨 증강 확률 조회
                var nextAugment = data.GetNextLevelData();
                augmentID = nextAugment.GetID();
                var probability = probabilitiesRecord[nextAugment.GetID()]; 
                probabilityInteger = (int)(probability.Probability * 1000); // Convert to percentage
            }
            else
            {
                // 인벤토리에 해당 옵션이 없다면, 1레렙 증강 조회
                var augment = augmentGroupByOption[option].First(a => a.GetLevel() == 1);
                augmentID = augment.GetID();
                var probability = probabilitiesRecord[augment.GetID()];
                probabilityInteger = (int)(probability.Probability * 1000); // Convert to percentage
            }
            
            var record = new ProbabilityRecord<int>
            {
                ID = augmentID,
                minProbability = totalProbability,
                maxProbability = totalProbability + probabilityInteger
            };
            probabilities.Add(record);
            totalProbability += probabilityInteger;
        }
        
        Debug.Log($"Total probability: {totalProbability}");
        // first class augment
        int randomIndex = UnityEngine.Random.Range(0, totalProbability);
        var selectedID0 = probabilities.FirstOrDefault(r => r.IsInRange(randomIndex));
        if (selectedID0.ID == 0)
        {
            Debug.LogError("No valid augment found. Please check the augment records.");
            return;
        }
        probabilities.Remove(selectedID0); // 중복 방지를 위해 제거
        do
        {
            randomIndex = UnityEngine.Random.Range(0, totalProbability);
        } while (probabilities.Any( r => r.IsInRange(randomIndex)) == false);
        var selectedID1 = probabilities.FirstOrDefault(r => r.IsInRange(randomIndex));
        if (selectedID1.ID == 0)
        {
            Debug.LogError("No valid augment found. Please check the augment records.");
            return;
        }
        
        Debug.Log($"Selected augment IDs: {selectedID0.ID}, {selectedID1.ID}");
        augmentSlots[0].SetAugment(ClassAugmentManager.Instance.GetAugment(selectedID0.ID));
        augmentSlots[1].SetAugment(ClassAugmentManager.Instance.GetAugment(selectedID1.ID));
    }
    
    private void RerollClassAugment()
    {
        Debug.Log("Reroll class augment.");
        var options = ClassAugmentManager.Instance.GetOptionsByClass(rerollAugmentType);
        GotchaClassAugment(options);
    }
    
    
    private AugmentRarity minRarity = AugmentRarity.None;
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
        augmentSlots[0].SetAugment(slot1);
        augmentSlots[1].SetAugment(slot2);
    }

    
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


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

public class ClassAugmentGotchaSystem : UIPage
{
    [Space(10), Header("Class Augment")]
    [SerializeField] private AugmentSlot[] classAugmentSlots;
    
    [Space(10), Header("Reroll Class Choice")]
    [SerializeField] private Button warriorSlot;
    [SerializeField] private Button wizardSlot;
    [SerializeField] private Button archerSlot;
    
    [Space(10), Header("Reroll Button")]
    [SerializeField] private Button classRerollAugmentButton;
    
    public override UIPageType UIPageType => UIPageType.ClassAumgentSelection;

    private IPageFlowManageable Owner;
    private bool IsRerollClassSelected = false;
    AugmentType rerollAugmentType = AugmentType.Warrior;
    
    public override void Initialize(IPageFlowManageable owner)
    {
        Owner = owner ?? throw new System.ArgumentNullException(nameof(owner), "Owner cannot be null.");
        classRerollAugmentButton.onClick.AddListener(() => {
            Debug.Log("Reroll augment button clicked.");
            // Handle reroll logic here
            RerollClassAugment();
        });
        
        warriorSlot.onClick.AddListener(() => {
            Debug.Log("Warrior slot selected.");
            IsRerollClassSelected = true;
            rerollAugmentType = AugmentType.Warrior;
            // Handle warrior slot selection logic here
        });
        wizardSlot.onClick.AddListener(() => {
            Debug.Log("Wizard slot selected.");
            IsRerollClassSelected = true;
            rerollAugmentType = AugmentType.Wizard;
            // Handle wizard slot selection logic here
        });
        archerSlot.onClick.AddListener(() => {
            Debug.Log("Archer slot selected.");
            IsRerollClassSelected = true;
            rerollAugmentType = AugmentType.Archer;
            // Handle archer slot selection logic here
        });
    }

    public override void Enter()
    {
        gameObject.SetActive(true);
        
        GotchaClassAugment( ClassAugmentManager.Instance.GetAllOption());
    }
    
    public override void Exit()
    {
        gameObject.SetActive(false);
        BattleManager.Instance.StartGame();
    }
    
    private void RerollClassAugment()
    {
        if (IsRerollClassSelected == false)
        {
            Debug.Log("Reroll class augment not selected yet. Please select a class first.");
            return;
        }
        
        Debug.Log("Reroll class augment.");
        var options = ClassAugmentManager.Instance.GetOptionsByClass(rerollAugmentType);
        GotchaClassAugment(options);
    }

    private void GotchaClassAugment(IReadOnlyList<int> options)
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
                Debug.Log($" Option {option} exists in inventory.");
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
        for(int i= 0 ; i < classAugmentSlots.Length; i++)
        {
            GotchaSlot(i, totalProbability, probabilities);
        }
    }
    
    private void GotchaSlot(int slotIndex, int totalProbability, List<ProbabilityRecord<int>> probabilities)
    {
        if (probabilities.Count == 0) // 더이상 뽑을 증강이 없다.
        {
            classAugmentSlots[slotIndex].gameObject.SetActive(false);
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
        classAugmentSlots[slotIndex].gameObject.SetActive(true);
        classAugmentSlots[slotIndex].SetAugment(ClassAugmentManager.Instance.GetAugment(selectedID.ID), OnSelectAugment);
    }
    
    private void OnSelectAugment(Augment augment)
    {
        BattleEventManager.CallEvent(new SelectAugmentEventArgs(augment));
        Owner.SwapPage(UIPageType.BattlePage);
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    
    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}
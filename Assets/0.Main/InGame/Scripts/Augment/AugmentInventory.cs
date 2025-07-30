using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class ClassAugmentUserData 
{
    public AugmentType AugmentType { get; set; }
    public int OptionID { get; set; }
    public int Level { get; set; }
    
    
    public ClassAugmentUserData() {}
    public ClassAugmentUserData(int optionId, int level)
    {
        OptionID = optionId;
        Level = level;
    }

    public ClassAugment GetData()
    {
        return ClassAugmentManager.Instance.GetAugmentWithOptionAndLevel(OptionID, Level);
    }
    
    public ClassAugment GetNextLevelData() 
    {
        if (IsMaxLevel())
        {
            Debug.LogWarning($" [ClassAugmentUserData] Cannot get next level data for at level {Level}. Already at max level.");
            return ClassAugmentManager.Instance.GetAugmentWithOptionAndLevel(OptionID, Level);
        }
        return ClassAugmentManager.Instance.GetAugmentWithOptionAndLevel(OptionID, Level + 1);
    }

    public bool IsMaxLevel()
    {
        return ClassAugmentManager.Instance.IsMaxLevel(OptionID, Level);
    }
    
    public void SetLevel(int level)
    {
        if (level < 1)
        {
            Debug.LogWarning($" [ClassAugmentUserData] Cannot set level to {level}. Level must be at least 1.");
            return;
        }
        Level = level;
    }

    public void NextLevel()
    {
        Level = Level + 1;
    }

    public override string ToString()
    {
        return $"{OptionID}:{Level}";
    }

    
}

[System.Serializable]
public class CommonAugmentUserData
{
    public int ID { get; set; }
    public AugmentType AugmentType { get; set; }
    public int OptionID { get; set; }
    public AugmentRarity Rarity { get; set; }
    public int Count { get; set; }
    
    public CommonAugment GetData()
    {
        return CommonAugmentManager.Instance.GetAugmentFiltered(Rarity, OptionID);
    }

    public CommonAugmentUserData() {}
    public CommonAugmentUserData(int id, int optionId, AugmentRarity rarity, int count)
    {
        ID = id;
        OptionID = optionId;
        Rarity = rarity; // Default value, can be set based on game logic
        Count = count; // Assuming represents the count of this augment
    }
}

public class AugmentInventory : MonoSingleton<AugmentInventory>
{
    [SerializeField] private int maxAugmentCount = 15; // Maximum number of augments that can be held
    public bool IsFull => (commonAugments.Count + warriorAugments.Count + archerAugments.Count + wizardAugments.Count) >= maxAugmentCount;
    
    List<CommonAugmentUserData> commonAugments = new List<CommonAugmentUserData>();
    public IReadOnlyList<CommonAugmentUserData> CommonAugments => commonAugments;
    
    List<ClassAugmentUserData> warriorAugments => classAugments[AugmentType.Warrior];
    public IReadOnlyList<ClassAugmentUserData> WarriorAugments => warriorAugments;
    
    List<ClassAugmentUserData> archerAugments => classAugments[AugmentType.Archer];
    public IReadOnlyList<ClassAugmentUserData> ArcherAugments => archerAugments;
    
    List<ClassAugmentUserData> wizardAugments => classAugments[AugmentType.Wizard];
    
    public IReadOnlyList<ClassAugmentUserData> WizardAugments => wizardAugments;
    
    Dictionary<AugmentType,List<ClassAugmentUserData>> classAugments = new Dictionary<AugmentType, List<ClassAugmentUserData>>()
    {
        { AugmentType.Warrior, new List<ClassAugmentUserData>() },
        { AugmentType.Archer, new List<ClassAugmentUserData>() },
        { AugmentType.Wizard, new List<ClassAugmentUserData>() }
    };

    private void Start()
    {
        BattleEventManager.RegistEvent<SelectAugmentEventArgs>(OnSelectAugment);
    }
    
    public void ClearInventory()
    {
        commonAugments.Clear();
        warriorAugments.Clear();
        archerAugments.Clear();
        wizardAugments.Clear();
        classAugments[AugmentType.Warrior].Clear();
        classAugments[AugmentType.Archer].Clear();
        classAugments[AugmentType.Wizard].Clear();
        
        Debug.Log("[AugmentInventory] Inventory cleared.");
        SaveData();
    }
    
    public void UpdateAugumetToInventory(Augment augment)
    {
        switch (augment.GetAugmentType())
        {
            case AugmentType.Common:
                var commonArgument = augment as CommonAugment;
                var commonExist = commonAugments.Any(a => a.ID == commonArgument.ID);
                if (commonExist == false)
                {
                    Debug.Log($"[AugmentInventory] Adding new Common Augment: {commonArgument.GetName()}");
                    var commonData = new CommonAugmentUserData(commonArgument.GetID(), commonArgument.GetOptionID(), commonArgument.Rarity, 1);
                    commonAugments.Add(commonData);
                }
                else
                {
                    Debug.Log($"[AugmentInventory] Upgrading existing Common Augment: {commonArgument.GetName()}");
                    var existing = commonAugments.First(a => a.ID == commonArgument.ID);
                    existing.Count++;
                }
                break;
            case AugmentType.Warrior:
            {
                var exist = warriorAugments.Any(a => a.OptionID == augment.GetOptionID());
                var classAugment = augment as ClassAugment;
                if (exist == false)
                {
                    Debug.Log($"[AugmentInventory] Adding new Warrior Augment: {augment.GetName()}");
                    var warriorData = new ClassAugmentUserData(augment.GetOptionID(), classAugment.GetLevel());
                    warriorAugments.Add(warriorData);
                }
                else
                {
                    Debug.Log($"[AugmentInventory] Upgrading existing Warrior Augment: {augment.GetName()}");
                    var existing = warriorAugments.First(a => a.OptionID == augment.GetOptionID());
                    existing.SetLevel(classAugment.GetLevel());
                }
                break;
            }
            case AugmentType.Archer:
            {
                var archerExist = archerAugments.Any(a => a.OptionID == augment.GetOptionID());
                var classAugment = augment as ClassAugment;
                if (archerExist == false)
                {
                    Debug.Log($"[AugmentInventory] Adding new Archer Augment: {augment.GetName()}");
                    var archerData = new ClassAugmentUserData(augment.GetOptionID(), classAugment.GetLevel());
                    archerAugments.Add(archerData);
                }
                else
                {
                    Debug.Log($"[AugmentInventory] Upgrading existing Archer Augment: {augment.GetName()}");
                    var existing = archerAugments.First(a => a.OptionID == augment.GetOptionID());
                    existing.SetLevel(classAugment.GetLevel());
                }
                break;
            }
            case AugmentType.Wizard:
            {
                var wizardExist = wizardAugments.Any(a => a.OptionID == augment.GetOptionID());
                var classAugment = augment as ClassAugment;
                if (wizardExist == false)
                {
                    Debug.Log($"[AugmentInventory] Adding new Wizard Augment: {augment.GetName()}");
                    var wizardData = new ClassAugmentUserData( augment.GetOptionID(), classAugment.GetLevel());
                    wizardAugments.Add(wizardData);
                }
                else
                {
                    Debug.Log($"[AugmentInventory] Upgrading existing Wizard Augment: {augment.GetName()}, {augment.GetDescription()}");
                    var existing = wizardAugments.First(a => a.OptionID == augment.GetOptionID());
                    existing.SetLevel(classAugment.GetLevel());
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
        SaveData();
    }

    private void OnSelectAugment(SelectAugmentEventArgs args)
    {
        Debug.Log("[AugmentInventory] OnSelectAugment called with data: " + args.Data);
        var augment = args.Data;
        UpdateAugumetToInventory(augment);
        
        Debug.Log($"Inventory Updated: " +
                  $"Common Augments: {commonAugments.Count}, " +
                  $"Warrior Augments: {warriorAugments.Count}, " +
                  $"Archer Augments: {archerAugments.Count}, " +
                  $"Wizard Augments: {wizardAugments.Count}");

        if (IsFull)
        {
            Debug.Log($" [AugmentInventory] Inventory is full. Cannot add more augments.");
        }


    }

    protected override void Initialize()
    {
        base.Initialize();

        void RegistUserData<T>(string backendData, List<T> userData)
        {
            if (string.IsNullOrEmpty(backendData)) return;
            
            var result = JsonReader.Read<List<T>>(backendData);
            userData.AddRange(result);
        }

        
        var es3Manager = ES3Manager.Instance;
        var es3UserAugmentData = es3Manager.AugmentData;
        RegistUserData(es3UserAugmentData.ArcherAugment, archerAugments);
        RegistUserData(es3UserAugmentData.WarriorAugment, warriorAugments);
        RegistUserData(es3UserAugmentData.WizardAugment, wizardAugments);
        RegistUserData(es3UserAugmentData.CommonAugment, commonAugments);
    }


    public void SaveData()
    {
        
        var es3Manager = ES3Manager.Instance;
        var backendAugmentData = es3Manager.AugmentData;
        backendAugmentData.ArcherAugment = JsonWriter.Write(archerAugments);
        backendAugmentData.WarriorAugment = JsonWriter.Write(warriorAugments);
        backendAugmentData.WizardAugment = JsonWriter.Write(wizardAugments);
        backendAugmentData.CommonAugment = JsonWriter.Write(commonAugments);
        es3Manager.SaveAumgents();
    }

    public List<ClassAugmentUserData> GetAllClassAugments()
    {
        List<ClassAugmentUserData> classAugments = new List<ClassAugmentUserData>();
        classAugments.AddRange(wizardAugments);
        classAugments.AddRange(archerAugments);
        classAugments.AddRange(warriorAugments);
        return classAugments;
    }

    public void ApplyAugmentsToSquad(Squad squad)
    {
        var status = squad.stats;
        foreach (var commonAugment in commonAugments)
        {
            var data = commonAugment.GetData();
            if (data == null)
            {
                Debug.LogWarning($"[AugmentInventory] Common Augment with ID {commonAugment.ID} not found.");
                continue;
            }
            var upgradeFactor = commonAugment.Count;
            status.MaxHealth += data.SquadMaxHpIncrease * upgradeFactor;
            status.AttackDamageFactor += data.AtkIncrease * upgradeFactor;
            status.DecreaseAttackSpeed += data.AtkSpeedDecrease * upgradeFactor;
            status.MoveSpeed += data.MoveSpeedIncrease * upgradeFactor;
            status.CriticalChance += data.CriticalRateIncrease * upgradeFactor;
            status.CriticalDamage += data.CriticalDamageIncrease * upgradeFactor;
            status.BonusDamagePerHit += data.AdditionalHit * upgradeFactor;
            status.TakeDamageFactor += data.IncreasedTakenDamage * upgradeFactor;
            status.FinalDamageFactor += data.IncreasedFinalDamage * upgradeFactor;
        
            Debug.Log($"[AugmentInventory] Applied Common Augment: {data.GetName()} Count {commonAugment.Count} to Squad.");
        }
        status.CurrentHealth = status.MaxHealth; // Reset current health to max after applying augments
    }

    public void ApplyAugmentsToArcher(Archer archer)
    {
        Debug.Log($"[AugmentInventory] Applying Augments {archerAugments.Count} to Archer...");
        foreach (var archerAugment in archerAugments)
        {
            var data = archerAugment.GetData() as ArcherAugment;
            if (data == null)
            {
                Debug.LogWarning($"[AugmentInventory] Archer Augment with {archerAugment} not found.");
                continue;
            }
            
            data.Apply(archer, archerAugment.IsMaxLevel());
            Debug.Log($"[AugmentInventory] Applied Archer Augment: {data.GetName()} to Archer.");
            
        }
    }

    public void ApplyAugmentsToWarrior(Warrior warrior)
    {
        foreach (var warriorAugment in warriorAugments)
        {
            var data = warriorAugment.GetData() as WarriorAugment;
            if (data == null)
            {
                Debug.LogWarning($"[AugmentInventory] Warrior Augment with {warriorAugment} not found.");
                continue;
            }
            
            data.Apply(warrior,warriorAugment.IsMaxLevel() );
            Debug.Log($"[AugmentInventory] Applied Warrior Augment: {data.GetName()} to Warrior.");
        }
    }

    public void ApplyAugmentsToWizard(Wizard wizard)
    {
        foreach (var wizardAugment in wizardAugments)
        {
            var data = wizardAugment.GetData() as WizardAugment;
            if (data == null)
            {
                Debug.LogWarning($"[AugmentInventory] Wizard Augment with {wizardAugment} not found.");
                continue;
            }
            
            data.Apply(wizard, wizardAugment.IsMaxLevel());
            Debug.Log($"[AugmentInventory] Applied Wizard Augment: {data.GetName()} to Wizard.");
        }
    }

}

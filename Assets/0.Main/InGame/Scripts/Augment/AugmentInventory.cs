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
    public int ID { get; set; }
    public AugmentType AugmentType { get; set; }
    public int OptionID { get; set; }
    public int Level { get; set; }
    
    
    public ClassAugmentUserData() {}
    public ClassAugmentUserData(int id, int optionId, int level)
    {
        ID = id;
        OptionID = optionId;
        Level = level;
    }

    public ClassAugment GetData()
    {
        return ClassAugmentManager.Instance.GetAugment(ID);
    }
    
    public ClassAugment GetNextLevelData() 
    {
        if (IsMaxLevel())
        {
            return null; // Cannot get next level data if already at max level
        }
        return ClassAugmentManager.Instance.GetAugment(ID, OptionID, Level + 1);
    }

    public bool IsMaxLevel()
    {
        return ClassAugmentManager.Instance.IsMaxLevel(OptionID, Level);
    }

    public void NextLevel()
    {
        Level = Level + 1;
        ID = ClassAugmentManager.Instance.GetAugmentWithOptionAndLevel(OptionID, Level).GetID();
    }

    public override string ToString()
    {
        return $"{ID}:{OptionID}:{Level}";
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
    
    List<ClassAugmentUserData> warriorAugments = new List<ClassAugmentUserData>();
    public IReadOnlyList<ClassAugmentUserData> WarriorAugments => warriorAugments;
    
    List<ClassAugmentUserData> archerAugments = new List<ClassAugmentUserData>();
    public IReadOnlyList<ClassAugmentUserData> ArcherAugments => archerAugments;
    
    List<ClassAugmentUserData> wizardAugments = new List<ClassAugmentUserData>();
    
    public IReadOnlyList<ClassAugmentUserData> WizardAugments => wizardAugments;

    private void Start()
    {
        BattleEventManager.Instance.Callbacks.OnSelectAugment += OnSelectAugment;
    }

    private void OnSelectAugment(SelectAugmentEventArgs args)
    {
        var augment = args.Data;

        switch (augment.GetAugmentType())
        {
            case AugmentType.Common:
                var commonArgument = augment as CommonAugment;
                var commonExist = commonAugments.Any(a => a.Rarity == commonArgument.Rarity && a.OptionID == commonArgument.OptionID);
                if (commonExist == false)
                {
                    var commonData = new CommonAugmentUserData(commonArgument.GetID(), commonArgument.GetOptionID(), commonArgument.Rarity, 1);
                    commonAugments.Add(commonData);
                }
                else
                {
                    var existing = commonAugments.First(a => a.ID == commonArgument.ID && a.OptionID == commonArgument.OptionID);
                    existing.Count++;
                }
                break;
            case AugmentType.Warrior:
                var exist = warriorAugments.Any(a => a.ID == augment.GetID() && a.OptionID == augment.GetOptionID());
                if (exist == false)
                {
                    var warriorData = new ClassAugmentUserData(augment.GetID(), augment.GetOptionID(), 1);
                    warriorAugments.Add(warriorData);
                }
                else
                {
                    var existing = warriorAugments.First(a => a.ID == augment.GetID() && a.OptionID == augment.GetOptionID());
                    existing.NextLevel();
                }
                break;
            case AugmentType.Archer:
                var archerExist = archerAugments.Any(a => a.ID == augment.GetID() && a.OptionID == augment.GetOptionID());
                if (archerExist == false)
                {
                    var archerData = new ClassAugmentUserData(augment.GetID(), augment.GetOptionID(), 1);
                    archerAugments.Add(archerData);
                }
                else
                {
                    var existing = archerAugments.First(a => a.ID == augment.GetID() && a.OptionID == augment.GetOptionID());
                    existing.NextLevel();
                }
                break;
            case AugmentType.Wizard:
                var wizardExist = wizardAugments.Any(a => a.ID == augment.GetID() && a.OptionID == augment.GetOptionID());
                if (wizardExist == false)
                {
                    var wizardData = new ClassAugmentUserData(augment.GetID(), augment.GetOptionID(), 1);
                    wizardAugments.Add(wizardData);
                }
                else
                {
                    var existing = wizardAugments.First(a => a.ID ==  augment.GetID() && a.OptionID == augment.GetOptionID());
                    existing.NextLevel();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Debug.Log($"Inventory Updated: " +
                  $"Common Augments: {commonAugments.Count}, " +
                  $"Warrior Augments: {warriorAugments.Count}, " +
                  $"Archer Augments: {archerAugments.Count}, " +
                  $"Wizard Augments: {wizardAugments.Count}");

        if (IsFull)
        {
            Debug.Log($" [AugmentInventory] Inventory is full. Cannot add more augments.");
        }

        SaveData();

    }

    protected override void Initialize()
    {
        base.Initialize();

        var classManager = ClassAugmentManager.Instance;

        void RegistUserData<T>(string backendData, List<T> userData)
        {
            var result = JsonReader.Read<List<T>>(backendData);
            userData.AddRange(result);
        }
        
        if(BackendAugmentData.augment == null)
        {
            Debug.LogWarning("BackendAugmentData.augment is null. Please check the backend data initialization.");
            return;
        }
        
        
        // Load Augment Data from Backend
        var archerData = BackendAugmentData.augment.ArcherAugmentData;
        RegistUserData(archerData, archerAugments);
        
        var warriorData = BackendAugmentData.augment.WarriorAugmentData;
        RegistUserData(warriorData, warriorAugments);
        
        var wizardData = BackendAugmentData.augment.WizardAugmentData;
        RegistUserData(wizardData, wizardAugments);
        
        var commonData = BackendAugmentData.augment.CommonAugmentData;
        RegistUserData(wizardData, commonAugments);
    }


    public void SaveData()
    {
        if (BackendAugmentData.augment == null)
        {
            return;
        }
        
        BackendAugmentData.augment.ArcherAugmentData = JsonWriter.Write(archerAugments);
        BackendAugmentData.augment.WarriorAugmentData = JsonWriter.Write(warriorAugments);
        BackendAugmentData.augment.WizardAugmentData = JsonWriter.Write(wizardAugments);
        BackendAugmentData.augment.CommonAugmentData = JsonWriter.Write(commonAugments);
    }


    public List<ClassAugmentUserData> GetAllClassAugments()
    {
        List<ClassAugmentUserData> classAugments = new List<ClassAugmentUserData>();
        classAugments.AddRange(wizardAugments);
        classAugments.AddRange(archerAugments);
        classAugments.AddRange(warriorAugments);
        return classAugments;
    }
}

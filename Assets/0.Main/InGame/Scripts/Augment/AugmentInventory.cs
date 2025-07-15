using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ClassAugmentUserData 
{
    public int ID { get; set; }
    public int OptionID { get; set; }
    public int Level { get; private set; }
    
    
    public ClassAugmentUserData(string line)
    {
        var properties = line.Split(":");
        ID = int.Parse(properties[0]); // ID
        OptionID = int.Parse(properties[1]); // OptionID
        Level = int.Parse(properties[2]); // Level
    }
    
    public ClassAugmentUserData(int id, int optionId, int level)
    {
        ID = id;
        OptionID = optionId;
        Level = level;
    }

    public bool IsMaxLevel()
    {
        return ClassAugmentManager.Instance.IsMaxLevel(ID, OptionID, Level);
    }

    public void NextLevel()
    {
        Level = Level + 1;
    }

    public override string ToString()
    {
        return $"{ID}:{OptionID}:{Level}";
    }
    
}

public class CommonAugmentUserData
{
    public int ID { get; set; }
    public int OptionID { get; set; }
    public AugmentRarity Rarity { get; set; }
    public int Count { get; set; }

    public CommonAugmentUserData(string line)
    {
        var properties = line.Split(":");
        ID = int.Parse(properties[0]); // ID
        OptionID = int.Parse(properties[1]); // OptionID
        Rarity = (AugmentRarity)Enum.Parse(typeof(AugmentRarity), properties[2]); // Rarity
        Count = int.Parse(properties[3]); // Count
    }
    
    public CommonAugmentUserData(int id, int optionId, AugmentRarity rarity, int count)
    {
        ID = id;
        OptionID = optionId;
        Rarity = rarity; // Default value, can be set based on game logic
        Count = count; // Assuming represents the count of this augment
    }

    public override string ToString()
    {
        return $"{ID}:{OptionID}:{Rarity}:{Count}";
    }
}

public class AugmentInventory : MonoSingleton<AugmentInventory>
{
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
                break;
            case AugmentType.Warrior:
                break;
            case AugmentType.Archer:
                break;
            case AugmentType.Wizard:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void Initialize()
    {
        base.Initialize();

        var classManager = ClassAugmentManager.Instance;

        void RegistClassUserData(string backendData, List<ClassAugmentUserData> userData)
        {
            var rawDatas = backendData.Split(",").ToList();
            foreach (var rawData in rawDatas)
            {
                var augment = new ClassAugmentUserData(rawData);
                userData.Add(augment);
            }
        }
        
        // Load Augment Data from Backend
        var archerData = BackendAugmentData.augment.ArcherAugmentData;
        RegistClassUserData(archerData, archerAugments);
        
        var warriorData = BackendAugmentData.augment.WarriorAugmentData;
        RegistClassUserData(warriorData, warriorAugments);
        
        var wizardData = BackendAugmentData.augment.WizardAugmentData;
        RegistClassUserData(wizardData, wizardAugments);
        
        var commonData = BackendAugmentData.augment.CommonAugmentData;
        var rawDatas = commonData.Split(",").ToList();
        foreach (var rawData in rawDatas)
        {
            var properties = rawData.Split(":");
            var augment = new CommonAugmentUserData(rawData);
            commonAugments.Add(augment);
        }
    }

}

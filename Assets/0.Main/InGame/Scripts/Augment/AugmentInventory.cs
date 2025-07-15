using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AugmentUserData<T> where T : Augment
{
    public int ID { get; set; }
    public int Count { get; set; }
    
    private IReadOnlyDictionary<int, T> augmentRecords;
    
    public T Data => augmentRecords[ID];
    
    public AugmentUserData(int id, int count , IReadOnlyDictionary<int, T> augmentRecords)
    {
        ID = id;
        Count = count;
        this.augmentRecords = augmentRecords;
    }
    
}

public class AugmentInventory : MonoSingleton<AugmentInventory>
{
    List<AugmentUserData<CommonAugment>> commonAugments = new List<AugmentUserData<CommonAugment>>();
    public IReadOnlyList<AugmentUserData<CommonAugment>> CommonAugments => commonAugments;
    List<AugmentUserData<WarriorAugment>> warriorAugments = new List<AugmentUserData<WarriorAugment>>();
    public IReadOnlyList<AugmentUserData<WarriorAugment>> WarriorAugments => warriorAugments;
    List<AugmentUserData<ArcherAugment>> archerAugments = new List<AugmentUserData<ArcherAugment>>();
    public IReadOnlyList<AugmentUserData<ArcherAugment>> ArcherAugments => archerAugments;
    List<AugmentUserData<WizardAugment>> wizardAugments = new List<AugmentUserData<WizardAugment>>();
    public IReadOnlyList<AugmentUserData<WizardAugment>> WizardAugments => wizardAugments;
    
    protected override void Initialize()
    {
        base.Initialize();

        var classManager = ClassAugmentManager.Instance;

        // Load Augment Data from Backend
        var archerData = BackendAugmentData.augment.ArcherAugmentData;
        if (string.IsNullOrEmpty(archerData))
        {
        }
        else
        {
            var records = classManager.ArcherAugments;
            var rawDatas = 
                archerData.Split(",").
                    Where( data => records.ContainsKey(int.Parse(data))).
                    Select(data => new AugmentUserData<ArcherAugment>(int.Parse(data),1, records)).
                    ToList();
          
            Debug.Log($"Archer Augment Data Loaded: {archerData}");
        }
        
        var warriorData = BackendAugmentData.augment.WarriorAugmentData;
        if (string.IsNullOrEmpty(warriorData))
        {
        }
        else
        {
            var records = classManager.WarriorAugments;
            warriorAugments = 
                warriorData.Split(",").
                    Where( data => records.ContainsKey(int.Parse(data))).
                    Select(data => new AugmentUserData<WarriorAugment>(int.Parse(data),1, records)).
                    ToList();
          
            Debug.Log($"Warrior Augment Data Loaded: {warriorData}");
        }
        
        var wizardData = BackendAugmentData.augment.WizardAugmentData;
        if (string.IsNullOrEmpty(wizardData))
        {
        }
        else
        {
            var records =  classManager.WizardAugments;
            wizardAugments = 
                wizardData.Split(",").
                    Where( data => records.ContainsKey(int.Parse(data))).
                    Select(data => new AugmentUserData<WizardAugment>(int.Parse(data), 1, records)).
                    ToList();
          
            Debug.Log($"Wizard Augment Data Loaded: {wizardData}");
        }
        
        var commonData = BackendAugmentData.augment.CommonAugmentData;
        if (string.IsNullOrEmpty(commonData))
        {
        }
        else
        {
            var records = CommonAugmentManager.Instance.Records;
            var rawDatas = 
                commonData.Split(",").
                    Where( data => records.ContainsKey(int.Parse(data))).
                    Select(data => records[int.Parse(data)]).
                    ToList();
          
            Debug.Log($"Common Augment Data Loaded: {commonData}");
        }
    }

}

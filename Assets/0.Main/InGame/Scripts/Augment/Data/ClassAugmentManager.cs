using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class ClassAugmentManager : MonoSingleton<ClassAugmentManager>
{
    [SerializeField] private string archerAugmentPath = "ArcherAugment";
    [SerializeField] private string warriorAugmentPath = "WarriorAugment";
    [SerializeField] private string wizardAugmentPath = "WizardAugment";

    [SerializeField] private ClassAugmentIconTableSAO augmentIconTableSAO;
    
    Dictionary<int, ArcherAugment> archerAugments = new Dictionary<int, ArcherAugment>();
    Dictionary<int, WarriorAugment> warriorAugments = new Dictionary<int, WarriorAugment>();
    Dictionary<int, WizardAugment> wizardAugments = new Dictionary<int, WizardAugment>();
    
    public IReadOnlyDictionary<int, ArcherAugment> ArcherAugments => archerAugments;
    public IReadOnlyDictionary<int, WarriorAugment> WarriorAugments => warriorAugments;
    public IReadOnlyDictionary<int, WizardAugment> WizardAugments => wizardAugments;
    
    List<ClassAugment> allAugments = new List<ClassAugment>();
    Dictionary<int,ClassAugment> allAugmentsByID = new Dictionary<int, ClassAugment>();
    public IReadOnlyList<ClassAugment> AllAugments => allAugments;
    public IReadOnlyDictionary<int, ClassAugment> AllAugmentsByID => allAugmentsByID;
    
    
    // 게임 시작할때는 직업 상관 없이 랜덤하게 두개 
    Dictionary<int, List<ClassAugment>> augmentGroupByOption = new Dictionary<int, List<ClassAugment>>();
    public IReadOnlyDictionary<int, List<ClassAugment>> AugmentGroupByOption => augmentGroupByOption;
    
    Dictionary<AugmentType , HashSet<int>> optionGroupByClass = new Dictionary<AugmentType,HashSet<int>>();
    public IReadOnlyDictionary<AugmentType, HashSet<int>> OptionGroupByClass => optionGroupByClass;

    public Sprite GetIcon(int augmentID)
    {
        if (augmentIconTableSAO == null)
        {
            Debug.LogError("AugmentIconTableSAO is not assigned.");
            return null;
        }
        
        var augment = GetAugment(augmentID);
        if (augment == null)
        {
            Debug.LogWarning($"Augment with ID {augmentID} not found.");
            return null;
        }
        
        var icon = augmentIconTableSAO.GetIconByAugmentID(augment.GetID());
        if (icon == null)
        {
            Debug.LogWarning($"Icon for augment with ID {augmentID} not found.");
        }
        
        return icon;
        
    }
    
    protected override void Initialize()
    {
        Debug.Log("Initializing ClassAugmentManager");
        base.Initialize();
        if (string.IsNullOrEmpty(archerAugmentPath) || 
            string.IsNullOrEmpty(warriorAugmentPath) || 
            string.IsNullOrEmpty(wizardAugmentPath))
        {
            throw new ArgumentNullException("Augment paths cannot be null or empty.");
        }

        var archerTsvList = TSVLoader.LoadTable<ArcherAugmentTSV>(archerAugmentPath);
        archerAugments = archerTsvList.ToDictionary(a => a.ID, ArcherAugmentFactory.CreateAugment);
        
        //var warriorTsvList = TSVLoader.LoadTable<WarriorAugmentTSV>(warriorAugmentPath);
        //warriorAugments = warriorTsvList.ToDictionary(a => a.ID, WarriorAugmentFactory.CreateAugment);
        
        var wizardTsvList = TSVLoader.LoadTable<WizardAugmentTSV>(wizardAugmentPath);
        wizardAugments = wizardTsvList.ToDictionary(a => a.ID, WizardAugmentFactory.CreateAugment);
        
        Debug.Log($"Successfully loaded augments: Archer({archerAugments.Count}), Warrior({warriorAugments.Count}), Wizard({wizardAugments.Count})");
        // Combine all augments into a single list and dictionary
        allAugments.AddRange(archerAugments.Values);
        allAugments.AddRange(warriorAugments.Values);
        allAugments.AddRange(wizardAugments.Values);
        foreach (var augment in allAugments)
        {
            if (!allAugmentsByID.ContainsKey(augment.GetID()))
            {
                allAugmentsByID.Add(augment.GetID(), augment);
            }
            else
            {
                Debug.LogWarning($"Duplicate augment ID found: {augment.GetID()}");
            }
            
            // Group by option ID
            if (!augmentGroupByOption.ContainsKey(augment.GetOptionID()))
            {
                augmentGroupByOption[augment.GetOptionID()] = new List<ClassAugment>();
            }
            augmentGroupByOption[augment.GetOptionID()].Add(augment);
            
            // Group by class type
            if (!optionGroupByClass.ContainsKey(augment.GetAugmentType()))
            {
                optionGroupByClass[augment.GetAugmentType()] = new HashSet<int>();
            }
            optionGroupByClass[augment.GetAugmentType()].Add(augment.GetOptionID());
            
        
        }
    }

    public IReadOnlyList<int> GetAllOption()
    {
        return augmentGroupByOption.Keys.ToList();
    }
    
    public IReadOnlyList<int> GetOptionsByClass(AugmentType type)
    {
        return optionGroupByClass[type].ToList();
    }
   
    public bool IsMaxLevel(int optionID, int level)
    {
        var next = level + 1;
        var result = allAugments.FirstOrDefault(a =>( 
            a.GetOptionID() == optionID && 
            a.GetLevel() == next) );
        
        return result == null;
    }

    public ClassAugment GetAugment(int id)
    {
        if (allAugmentsByID.TryGetValue(id, out var augment))
        {
            return augment;
        }
        Debug.LogWarning($"Augment with ID {id} not found.");
        return null;
    }

    public ClassAugment GetAugmentWithOptionAndLevel(int optionID, int level)
    {
        var result = allAugments.FirstOrDefault(a => 
            a.GetOptionID() == optionID && 
            a.GetLevel() == level);
        return result;
    }
   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class ClassAugmentManager : SingletonBase<ClassAugmentManager> , ILoadable
{
    [SerializeField] private string archerAugmentPath = "ArcherAugment";
    [SerializeField] private string warriorAugmentPath = "WarriorAugment";
    [SerializeField] private string wizardAugmentPath = "WizardAugment";

    
    public bool IsLoaded => ArcherAugmentManager.IsLoaded && WarriorAugmentManager.IsLoaded && WizardAugmentManager.IsLoaded;
    private InlineArcherAugmentManager ArcherAugmentManager = new InlineArcherAugmentManager();
    private InlineWarriorAugmentManager WarriorAugmentManager = new InlineWarriorAugmentManager();
    private InlineWizardAugmentManager WizardAugmentManager = new InlineWizardAugmentManager();
    
    public IReadOnlyDictionary<int, ArcherAugment> ArcherAugments => ArcherAugmentManager.Records;
    public IReadOnlyDictionary<int, WarriorAugment> WarriorAugments => WarriorAugmentManager.Records;
    public IReadOnlyDictionary<int, WizardAugment> WizardAugments => WizardAugmentManager.Records;
    
    List<ClassAugment> AllAugments;
    
    // 게임 시작할때는 직업 상관 없이 랜덤하게 두개 
    Dictionary<int, List<ClassAugment>> augmentGroupByOption = new Dictionary<int, List<ClassAugment>>();
    public IReadOnlyDictionary<int, List<ClassAugment>> AugmentGroupByOption => augmentGroupByOption;
    
    Dictionary<AugmentType , HashSet<int>> optionGroupByClass = new Dictionary<AugmentType,HashSet<int>>();
    public IReadOnlyDictionary<AugmentType, HashSet<int>> OptionGroupByClass => optionGroupByClass;
    
    Dictionary<int, List<ClassAugment>> augmentGroupByLevel = new Dictionary<int, List<ClassAugment>>();
    
    public void Load(Action<LoadCompleteEventArg> onComplete = null)
    {
        throw new NotImplementedException();
    }

    public ClassAugment NextAugment(ClassAugment augment)
    {
        var currentLevel = augment.GetLevel();
        var nextLevel = currentLevel + 1;

        var result = AllAugments.FirstOrDefault( a => 
            a.GetAugmentType() == augment.GetAugmentType() &&
            a.GetLevel() == nextLevel && 
            a.GetOptionID() == augment.GetOptionID());

        return null;
    }
    

    public async Task<LoadCompleteEventArg> LoadAsync()
    {
        ArcherAugmentManager = new InlineArcherAugmentManager(archerAugmentPath);
        WarriorAugmentManager = new InlineWarriorAugmentManager(warriorAugmentPath);
        WizardAugmentManager = new InlineWizardAugmentManager(wizardAugmentPath);
        
        var archerResult = await ArcherAugmentManager.LoadAsync();
        if (!archerResult.Success)
        {
            return new LoadCompleteEventArg(false, "Failed to load Archer Augments: " + archerResult.ErrorMessage);
        }
        
        var warriorResult = await WarriorAugmentManager.LoadAsync();
        if (!warriorResult.Success)
        {
            return new LoadCompleteEventArg(false, "Failed to load Warrior Augments: " + warriorResult.ErrorMessage);
        }
        
        var wizardResult = await WizardAugmentManager.LoadAsync();
        if (!wizardResult.Success)
        {
            return new LoadCompleteEventArg(false, "Failed to load Wizard Augments: " + wizardResult.ErrorMessage);
        }

        if (IsLoaded)
        {
            void RegistSubTable<T>(IReadOnlyDictionary<int, T> records)  where T : ClassAugment
            {
                foreach (var record in records.Values)
                {
                    AllAugments.Add(record);
                    
                    if (!augmentGroupByOption.ContainsKey(record.GetOptionID()))
                    {
                        augmentGroupByOption[record.GetOptionID()] = new List<ClassAugment>();
                    }
                    augmentGroupByOption[record.GetOptionID()].Add(record);
                
                    if (!optionGroupByClass.ContainsKey(record.GetAugmentType()))
                    {
                        optionGroupByClass[record.GetAugmentType()] = new HashSet<int>();
                    }
                    optionGroupByClass[record.GetAugmentType()].Add(record.GetID());
                    
                    if (!augmentGroupByLevel.ContainsKey(record.GetLevel()))
                    {
                        augmentGroupByLevel[record.GetLevel()] = new List<ClassAugment>();
                    }
                    augmentGroupByLevel[record.GetLevel()].Add(record);
                }
            }
            
            RegistSubTable(ArcherAugments);
            RegistSubTable(WarriorAugments);
            RegistSubTable(WizardAugments);
        }

        return new LoadCompleteEventArg(IsLoaded);
    }
    
    private class InlineWarriorAugmentManager : GenericDictionaryResourceManager<WarriorAugment, int, InlineWarriorAugmentManager>
    {
        public InlineWarriorAugmentManager(string resourcePath)
        {
            this.resourcePath = resourcePath;
        }

        public InlineWarriorAugmentManager()
        {
        }


        protected override int GetKey(WarriorAugment record)
        {
            return record.ID;
        }
    }
    
    private class InlineWizardAugmentManager : GenericDictionaryResourceManager<WizardAugment, int, InlineWizardAugmentManager>
    {
        public InlineWizardAugmentManager(string resourcePath )
        {
            this.resourcePath = resourcePath;
        }

        public InlineWizardAugmentManager()
        {
        }

        protected override int GetKey(WizardAugment record)
        {
            return record.ID;
        }
    }
    
    
    private class InlineArcherAugmentManager : GenericDictionaryResourceManager<ArcherAugment, int, InlineArcherAugmentManager>
    {
        public InlineArcherAugmentManager(string resourcePath)
        {
            this.resourcePath = resourcePath;
        }

        public InlineArcherAugmentManager() {}

        protected override int GetKey(ArcherAugment record)
        {
            return record.ID;
        }
    }

    public bool IsMaxLevel(int id, int optionID, int level)
    {
        var next = level + 1;
        var result = AllAugments.FirstOrDefault(a =>( 
            a.GetOptionID() == optionID && 
            a.GetLevel() == next) );
        
        return result == null;
    }
}

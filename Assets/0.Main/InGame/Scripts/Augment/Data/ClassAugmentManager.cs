using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AugmentOptionGroup
{
    private static int s_idGenerator = 1;
    public AugmentOptionGroup()
    {
        OptionID = s_idGenerator++;
    }
    public int OptionID { get; set; } 
    
    public void Add(ClassAugment augment)
    {
        AugmentSet.Add(augment);
        
    }
    public HashSet<ClassAugment> AugmentSet { get; set; } = new HashSet<ClassAugment>();

    public bool IsIn(int id)
    {
        foreach (var augment in AugmentSet)
        {
            if (augment.Next() == id)
            {
                return true;
            }
            
        }
        return false;
    }
    
}

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
    
    public void Load(Action<LoadCompleteEventArg> onComplete = null)
    {
        throw new NotImplementedException();
    }
    
    List<AugmentOptionGroup> augmentOptionGroups = new List<AugmentOptionGroup>();
    public IReadOnlyList<AugmentOptionGroup> AugmentOptionGroups => augmentOptionGroups;

    
    private AugmentOptionGroup SearchGroupOrNull(ClassAugment augment)
    {
        foreach (var group in augmentOptionGroups)
        {
            if (group.IsIn(augment.Next()))
            {
                return group;
            }
        }
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
            void UpdateGroup<T>(List<T> augments) where T : ClassAugment
            {
                foreach (var augment in augments)
                {
                    var group = SearchGroupOrNull(augment);
                    
                    if (group == null)
                    {
                        group = new AugmentOptionGroup();
                        augmentOptionGroups.Add(group);
                    }
                    
                    group.Add(augment);
                }
            }
            
            UpdateGroup(ArcherAugments.Values.ToList());
            UpdateGroup(WarriorAugments.Values.ToList());
            UpdateGroup(WizardAugments.Values.ToList());
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
    
}

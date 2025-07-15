using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AugmentRarityProbability
{
    public AugmentRarity Rarity { get; set; }
    public float Probability { get; set; }
}

public class AugmentOptionProbability
{
    public int OptionID { get; set; }
    public float Probability { get; set; }
}

public class ClassAugmentProbability
{
    public int  AugmentID { get; set; }
    public string AugmentName { get; set; }
    public int  AugmentLevel { get; set; }
    public float Probability { get; set; }
}

public class AugmentProbability : MonoSingleton<AugmentProbability> , ILoadable
{
    public Dictionary<AugmentRarity, AugmentRarityProbability> rarityRecords = new Dictionary<AugmentRarity, AugmentRarityProbability>();
    public Dictionary<int, AugmentOptionProbability> commonOptionRecords = new Dictionary<int, AugmentOptionProbability>();
    public Dictionary<int, ClassAugmentProbability> classOptionRecords = new Dictionary<int, ClassAugmentProbability>();
    
    [SerializeField] private string rarityPath = "AugmentProbability";
    [SerializeField] private string commonOptionPath = "CommonAugmentOptionProbability";
    [SerializeField] private string classOptionPath = "ClassAugmentProbability";
    
    bool isLoaded = false;
    public bool IsLoaded => isLoaded;
    public void Load(Action<LoadCompleteEventArg> onComplete = null)
    {
    }

    public async Task<LoadCompleteEventArg> LoadAsync()
    {
        isLoaded = true;
        {
            var result = await TSVLoader.LoadTableAsync<AugmentRarityProbability>(rarityPath);
            if (result != null)
            {
                rarityRecords = result.ToDictionary(r => r.Rarity, r => r);
            }
            else
            {
                Debug.LogError("Failed to load Augment Rarity Probability data from " + rarityPath);
                isLoaded = false;
            }
        }
        
        {
            var result = await TSVLoader.LoadTableAsync<AugmentOptionProbability>(commonOptionPath);
            if (result != null)
            {
                commonOptionRecords = result.ToDictionary(r => r.OptionID, r => r);
            }
            else
            {
                Debug.LogError("Failed to load Common Augment Option Probability data from " + commonOptionPath);
                isLoaded = false;
            }
        }
        
        {
            var result = await TSVLoader.LoadTableAsync<ClassAugmentProbability>(classOptionPath);
            if (result != null)
            {
                classOptionRecords = result.ToDictionary(r => r.AugmentID, r => r);
            }
            else
            {
                Debug.LogError("Failed to load Class Augment Probability data from " + classOptionPath);
                isLoaded = false;
            }
        }
        
        return new LoadCompleteEventArg(isLoaded); 
        
        
    }
}

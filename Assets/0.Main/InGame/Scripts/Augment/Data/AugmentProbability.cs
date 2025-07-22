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

public class AugmentProbability : MonoSingleton<AugmentProbability> 
{
    public Dictionary<AugmentRarity, AugmentRarityProbability> rarityRecords = new Dictionary<AugmentRarity, AugmentRarityProbability>(  );
    public Dictionary<int, AugmentOptionProbability> commonOptionRecords = new Dictionary<int, AugmentOptionProbability>();
    public Dictionary<int, ClassAugmentProbability> classRecords = new Dictionary<int, ClassAugmentProbability>();
    
    [SerializeField] private string rarityPath = "AugmentProbability";
    [SerializeField] private string commonOptionPath = "CommonAugmentOptionProbability";
    [SerializeField] private string classOptionPath = "ClassAugmentProbability";


    protected override void Initialize()
    {
        base.Initialize();
        if (string.IsNullOrEmpty(rarityPath) || string.IsNullOrEmpty(commonOptionPath) || string.IsNullOrEmpty(classOptionPath))
        {
            throw new ArgumentNullException("Resource paths cannot be null or empty.");
        }
        rarityRecords = TSVLoader.LoadTableToDictionary<AugmentRarity,AugmentRarityProbability>(rarityPath, r => r.Rarity);
        commonOptionRecords = TSVLoader.LoadTableToDictionary<int, AugmentOptionProbability>(commonOptionPath, r => r.OptionID);
        classRecords = TSVLoader.LoadTableToDictionary<int, ClassAugmentProbability>(classOptionPath, r => r.AugmentID);
    }
    
}

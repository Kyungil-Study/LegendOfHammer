using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommonAugmentManager : MonoSingleton<CommonAugmentManager>
{
    
    public IReadOnlyDictionary<int,CommonAugment> Records
    {
        get;
        private set;
    } = new Dictionary<int, CommonAugment>();

    [SerializeField] private string resourcePath = "CommonAugmentData";
    [SerializeField] private CommonAugmentIconTableSAO augmentIconTable;
    
    public Sprite GetIcon(int OptionId)
    {
        if (augmentIconTable == null)
        {
            Debug.LogError("CommonAugmentIconTableSAO is not assigned.");
            return null;
        }
        
       var icon = augmentIconTable.GetIconByOptionID(OptionId);
        if (icon == null)
        {
            Debug.LogWarning($"Icon for CommonAugment with ID {OptionId} not found.");
        }
        
        return icon;
    }
    
    public CommonAugment GetAugmentFiltered(AugmentRarity rarity, int optionID)
    {
        return Records.Values.FirstOrDefault(augment => 
            augment.Rarity == rarity && 
            augment.OptionID == optionID
            );
    }

    protected override void Initialize()
    {
        base.Initialize();
        var resultList = TSVLoader.LoadTableToDictionary<int, CommonAugmentTSV>(resourcePath, augment => augment.ID);
        
    }

    
}


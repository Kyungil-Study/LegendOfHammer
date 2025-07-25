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
        Records = TSVLoader.LoadTableToDictionary<int, CommonAugment>(resourcePath, augment => augment.ID);
    }

    
}


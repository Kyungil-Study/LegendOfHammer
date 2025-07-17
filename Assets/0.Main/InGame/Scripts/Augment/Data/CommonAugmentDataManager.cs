using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommonAugmentManager : GenericDictionaryResourceManager< CommonAugment,int, CommonAugmentManager>
{
    protected override int GetKey(CommonAugment record)
    {
        return record.ID;
    }

    public virtual void Load(Action<LoadCompleteEventArg> onComplete)
    {
        base.Load(onComplete);
    }
    
    public CommonAugment GetAugmentFiltered(AugmentRarity rarity, int optionID)
    {
        return Records.Values.FirstOrDefault(augment => 
            augment.Rarity == rarity && 
            augment.OptionID == optionID
            );
    }
    
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CommonAugmentProbability : GenericDictionaryResourceManager< CommonAugmentProbabilityData, int, CommonAugmentProbability>
{
    public Dictionary<string , IReadOnlyList<CommonAugmentProbabilityData>> AugmentProbabilityByRarity 
        = new Dictionary<string, IReadOnlyList<CommonAugmentProbabilityData>>();
    protected override int GetKey(CommonAugmentProbabilityData record)
    {
        return record.ID;
    }

    public override void Load(Action<LoadCompleteEventArg> onComplete = null)
    {
        base.Load(onComplete);
        if (isLoaded)
        {
            var list = Records.Values;
            foreach (var record in list)
            {
                if (!AugmentProbabilityByRarity.ContainsKey(record.AugmentRarity))
                {
                    AugmentProbabilityByRarity[record.AugmentRarity] = new List<CommonAugmentProbabilityData>();
                }
                ((List<CommonAugmentProbabilityData>)AugmentProbabilityByRarity[record.AugmentRarity]).Add(record);
            }
        }
    }
}


public class CommonAugmentProbabilityData
{
    public int ID { get; set; }
    public string AugmentName { get; set; }
    public string AugmentRarity { get; set; }
    public float ProbabilityWithinSameGrade { get; set; }
    public float FinalProbability { get; set; }
}

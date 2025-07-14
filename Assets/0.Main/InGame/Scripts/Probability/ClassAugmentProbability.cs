using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassAugmentProbability : GenericDictionaryResourceManager<ClassAugmentProbabilityData, int, ClassAugmentProbability>
{
    protected override int GetKey(ClassAugmentProbabilityData record)
    {
        return record.ID;
    }
}

public class ClassAugmentProbabilityData
{
    public int ID { get; set; }
    public string AugmentName { get; set; }
    public int AugmentLevel { get; set; }
    public float Probability { get; set; }
}

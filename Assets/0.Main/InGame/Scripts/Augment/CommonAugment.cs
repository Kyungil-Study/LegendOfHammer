using UnityEngine;

public class CommonAugmentTSV
{
    public int ID { get; set; }
    public string AugmentName { get; set; }
    public int OptionID { get; set; }
    public AugmentRarity Rarity { get; set; }
    public string Description { get; set; }

    public float AtkIncrease { get; set; }
    public float AtkSpeedDecrease { get; set; }
    public float CriticalRateIncrease { get; set; }
    public float CriticalDamageIncrease { get; set; }
    public int AdditionalHit { get; set; }
    public int SquadMaxHpIncrease { get; set; }
    public float IncreasedTakenDamage { get; set; }
    public float IncreasedFinalDamage { get; set; }
    public float MoveSpeedIncrease { get; set; }
}

public class CommonAugment : Augment
{
    public int ID { get; set; }
    public string AugmentName { get; set; }
    public int OptionID { get; set; }
    public AugmentRarity Rarity { get; set; }
    public string Description { get; set; }
    
    public float AtkIncrease { get; set; }
    public float AtkSpeedDecrease { get; set; }
    public float CriticalRateIncrease { get; set; }
    public float CriticalDamageIncrease { get; set; }
    public int AdditionalHit { get; set; }
    public int SquadMaxHpIncrease { get; set; }
    public float IncreasedTakenDamage { get; set; }
    public float IncreasedFinalDamage { get; set; }
    public float MoveSpeedIncrease { get; set; }


    public override int GetID()
    {
        return ID;
    }

    public override string GetName()
    {
        return AugmentName;
    }

    public override AugmentType GetAugmentType()
    {
        return AugmentType.Common;
    }

    public override string GetGrade()
    {
        return Rarity.ToString();
    }

    public override string GetDescription()
    {
        return Description;
    }

    public override int GetOptionID()
    {
        return OptionID;
    }

    public override Sprite GetIcon()
    {
        return ClassAugmentManager.Instance.GetIcon(OptionID);
    }
}


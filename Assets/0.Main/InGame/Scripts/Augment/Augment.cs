using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AugmentType
{
    Common,
    Warrior,
    Archer,
    Wizard
}

public enum AugmentRarity
{
    None,
    Normal,
    Rare,
    Hero,
    Legendary
}

public abstract class Augment
{
    public abstract  int GetID();
    public abstract string GetName();
    public abstract AugmentType GetAugmentType();
    public abstract string GetGrade();

    public abstract string GetDescription();
    
    public abstract int GetOptionID();
}

public abstract class ClassAugment : Augment
{
    public abstract int GetLevel();
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
}

public class WarriorAugment : ClassAugment
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int OptionID { get; set; }

    public int Level { get; set; }
    public string Description { get; set; }
    public string FinalDescription { get; set; }

    
    public int DashCoolDown { get; set; }
    public int KnockbackBonus { get; set; }
    public int ArcWaveSize { get; set; }
    public float ArcWaveAttackPowerRatio { get; set; }
    public float ExcuteHpRatio { get; set; }
    public float CollisionHpRatio { get; set; }
    public int DashCharge { get; set; }
    public string FinalUpgrade { get; set; }
    public override int GetID()
    {
        return ID;
    }

    public override string GetName()
    {
        return Name;
    }

    public override AugmentType GetAugmentType()
    {
        return AugmentType.Warrior;
    }
    
    public override string GetGrade()
    {
        return $"Lv.{Level} "; // Assuming Warrior Augments are always Hero grade
    }

    public override string GetDescription()
    {
        return Description; // Assuming Description is a string that describes the augment
    }

    public override int GetLevel()
    {
        return Level; // Assuming Level is an integer that represents the level of the augment
    }

    public override int GetOptionID()
    {
        return OptionID; // Assuming OptionID is the identifier for the augment option
    }
}

public class ArcherAugment : ClassAugment
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int OptionID { get; set; }

    public int Level { get; set; }
    public string Description { get; set; }
    public string FinalDescription { get; set; }

    // 공격속도 증가율
    public float AttackSpeedIncreasedRate { get; set; }
    // 소형화살 공격력 계수
    public float SmallArrowAttackRatio { get; set; }
    
    // 마법사 공격속도 증가율 
    public float MageAttackSpeedIncreased { get; set; }
    
    // 추가 투사체 공격력 계수 증가
    public float AdditionalProjectileAttackRatioIncreased { get; set; }
    // 추가 화살 공격력 계수 // 중복되는건지 확인 필요
    public float AdditionalArrowAttackRatio { get; set; }
    // 관통 횟수 증가
    public int PenetrationIncreased { get; set; }
    // 목표 대상 추가 피해 계수
    public float TargetAdditionalDamageRatio  { get; set; }
    public string FinalUpgrade { get; set; }
    
    public override int GetID()
    {
        return ID;
    }
    
    public override string GetName()
    {
        return Name;
    }
    
    public override AugmentType GetAugmentType()
    {
        return AugmentType.Archer;
    }
    public override string GetGrade()
    {
        return $"Lv.{Level} "; // Assuming Archer Augments are always Hero grade
    }

    public override string GetDescription()
    {
        return Description; // Assuming Description is a string that describes the augment
    }

    public override int GetLevel()
    {
        return Level; // Assuming Level is an integer that represents the level of the augment
    }

    public override int GetOptionID()
    {
        return OptionID; // Assuming OptionID is the identifier for the augment option
    }
}

public class WizardAugment : ClassAugment
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int OptionID { get; set; }
    public int Level { get; set; }
    public string Description { get; set; }
    public string FinalDescription { get; set; }
    
    public int Increased_Projectiles { get; set; }
    public float AttackRatio_ReductionRate  { get; set; }
    public float OverlapDamage_Reduction  { get; set; }
    public float Incresed_ExplosiveRange { get; set; }
    public float Dot_HP_Ratio_Duration  { get; set; }
    public float Dot_HP_Ratio  { get; set; }
    public float Debuff_Duration { get; set; }
    public float Debuff_Rate { get; set; }
    public float AdditionalExplosion  { get; set; }
    public float AdditionalExplosion_Ratio { get; set; }
    
    
    public override int GetID()
    {
        return ID;
    }
    
    public override string GetName()
    {
        return Name;
    }
    
    public override AugmentType GetAugmentType()
    {
        return AugmentType.Wizard;
    }
    
    public override string GetGrade()
    {
        return $"Lv.{Level} "; // Assuming Wizard Augments are always Hero grade
    }

    public override string GetDescription()
    {
        return Description; // Assuming Description is a string that describes the augment
    }

    public override int GetLevel()
    {
        return Level; // Assuming Level is an integer that represents the level of the augment
    }

    public override int GetOptionID()
    {
        return OptionID; // Assuming OptionID is the identifier for the augment option
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AugmentType
{
    Common = 0,
    Warrior = 1,
    Archer = 2,
    Wizard = 3
}

public enum AugmentRarity
{
    Common = 0,
    Rare = 1,
    Hero = 2,
    Legendary = 3
}

public abstract class Augment
{
    public abstract  int GetID();
    public abstract string GetName();
    public abstract AugmentType GetAugmentType();
    public abstract string GetGrade();


}

public class CommonAugment : Augment
{
    public int ID { get; set; }
    public string AugmentName { get; set; }
    public AugmentRarity Rarity { get; set; }
    public float AtkIncrease { get; set; }
    public float AtkSpeedDecrease { get; set; }
    public float CriticalRateIncrease { get; set; }
    public float CriticalDamageIncrease { get; set; }
    public int AdditionalHit { get; set; }
    public int SquadMaxHpIncrease { get; set; }
    public float IncreasedTakenDamage  { get; set; }
    public float IncreasedFinalDamage  { get; set; }
    public float MoveSpeedIncrease  { get; set; }

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
}

public class WarriorAugment : Augment
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int Level { get; set; }
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
}

public class ArcherAugment : Augment
{
    public int ID { get; set; }
    public string Name { get; set; }
    
    public int NextAugmentID { get; set; }
    public int Level { get; set; }
    public float AttackSpeedIncreasedRate { get; set; }
    public float SmallArrowAttackRatio { get; set; }
    public float AttackSpeedIncreased { get; set; }
    public float AdditionalProjectileAttackRatioIncreased { get; set; }
    public float AdditionalArrowAttackRatio { get; set; }
    public int PenetrationIncreased { get; set; }
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
}

public class WizardAugment : Augment
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int Level { get; set; }
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
    
    public string Final_Upgrade { get; set; }
    
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
    
}
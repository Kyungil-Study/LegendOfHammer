
using UnityEngine;

public static class WizardAugmentFactory
{
    public static WizardAugment CreateAugment(WizardAugmentTSV tsv)
    {
        return tsv.OptionID switch
        {
            9001 => new WizardProjectileAugment(tsv),
            9002 => new WizardExplosiveAugment(tsv),
            9003 => new WizardDotAugment(tsv),
            _ => throw new System.ArgumentException($"Unknown augment type: {tsv.OptionID}")
        };
    }

}

public class WizardProjectileAugment : WizardAugment
{
    public int Increased_Projectiles { get; set; } //개수
    public float AttackRatio_ReductionRate  { get; set; } 
    public float OverlapDamage_Reduction  { get; set; }

    
    public WizardProjectileAugment(WizardAugmentTSV tsv) : base(tsv)
    {
        Increased_Projectiles = tsv.Increased_Projectiles;
        AttackRatio_ReductionRate = tsv.AttackRatio_ReductionRate;
        OverlapDamage_Reduction = tsv.OverlapDamage_Reduction;
    }

    public override void Apply(Wizard wizard, bool isFinalUpgrade)
    {
        Debug.Log("마법사 개수 증가 선택");
        wizard.AttackCount = Increased_Projectiles;
        if (isFinalUpgrade)
        {
            wizard.baseAttackDamage = (int)(wizard.baseAttackDamage * AttackRatio_ReductionRate);
        }
    }
}

public class WizardDotAugment : WizardAugment
{
    public float Debuff_Duration { get; set; }
    public float Debuff_Rate { get; set; }
    
    public float AdditionalExplosion  { get; set; }
    
    public float AdditionalExplosion_Ratio { get; set; }
    
    public WizardDotAugment(WizardAugmentTSV tsv) : base(tsv)
    {
        Debuff_Duration = tsv.Debuff_Duration;
        Debuff_Rate = tsv.Debuff_Rate;
        AdditionalExplosion = tsv.AdditionalExplosion;
        AdditionalExplosion_Ratio = tsv.AdditionalExplosion_Ratio;
    }

    public override void Apply(Wizard wizard, bool isFinalUpgrade)
    {
        Debug.Log("마법사 디버프 선택");
        wizard.FinalDebuff = isFinalUpgrade;
        wizard.DebuffDuration = Debuff_Duration;
        wizard.DebuffRate = Debuff_Rate;
    }
}

public class WizardExplosiveAugment : WizardAugment
{
    public float Dot_HP_Ratio_Duration  { get; set; }
    public float Dot_HP_Ratio  { get; set; }
    public float Incresed_ExplosiveRange { get; set; }
    
    public WizardExplosiveAugment(WizardAugmentTSV tsv) : base(tsv)
    {
        Incresed_ExplosiveRange = tsv.Incresed_ExplosiveRange;
        Dot_HP_Ratio_Duration = tsv.Dot_HP_Ratio_Duration;
        Dot_HP_Ratio = tsv.Dot_HP_Ratio;
    }

    public override void Apply(Wizard wizard, bool isFinalUpgrade)
    {
        Debug.Log("마법사 폭발범위증가 선택");
        wizard.FinalExplosive = isFinalUpgrade;
        
        wizard.CurrentExplosionRadius = wizard.ExplosionRadius * Incresed_ExplosiveRange;
    }

}


public abstract class WizardAugment : ClassAugment
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int OptionID { get; set; }
    public int Level { get; set; }
    public string Description { get; set; }
    public string FinalDescription { get; set; }

    public sealed override void Apply(Hero hero, bool isFinalUpgrade)
    {
        Apply(hero as Wizard, isFinalUpgrade);
    }

    public abstract void Apply(Wizard wizard, bool isFinalUpgrade);

    public WizardAugment(WizardAugmentTSV tsv)
    {
        ID = tsv.ID;
        Name = tsv.Name;
        OptionID = tsv.OptionID;
        Level = tsv.Level;
        Description = tsv.Description;
        FinalDescription = tsv.FinalDescription;
    }

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
        return $"Lv.{Level} ";
    }

    public override string GetDescription()
    {
        return Description;
    }

    public override int GetOptionID()
    {
        return OptionID;
    }

    public override int GetLevel()
    {
        return Level;
    }
    
        
}
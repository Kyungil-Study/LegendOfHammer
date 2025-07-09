using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentTSV
{
    public int Augment_ID { get; set; }
    public string Augment_Name { get; set; }
    public int Augment_Level { get; set; }
}

public class WarriorAugmentTSV : AugmentTSV
{
    public int Dash_CoolDown { get; set; }
    public int Knockback_Bonus { get; set; }
    public int ArcWave_Size { get; set; }
    public float ArcWave_AttackPowerRatio { get; set; }
    public float Excute_HP_Ratio { get; set; }
    public float Collision_HP_Ratio { get; set; }
    public int Dash_Charge { get; set; }
    public int Final_Upgrade { get; set; }
}

public class ArcherAugmentTSV : AugmentTSV
{
    public float AttackSpeed_IncreasedRate { get; set; }
    public float SmallArrow_AttackRatio { get; set; }
    public float AttackSpeed_Increased { get; set; }
    public float AdditionalProjectile_AttackRatio_increased { get; set; }
    public float AdditionalArrow_AttackRatio { get; set; }
    public int Penetration_Increased { get; set; }
    public float Target_AdditionalDamageRatio  { get; set; }
    public string Final_Upgrade { get; set; }
}

public class WizardAugmentTSV : AugmentTSV
{
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
    
}
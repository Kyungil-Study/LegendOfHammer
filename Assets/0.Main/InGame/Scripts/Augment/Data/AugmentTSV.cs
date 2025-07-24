using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class WarriorAugmentTSV
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
    
}


public class ArcherAugmentTSV
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int OptionID { get; set; }

    public int Level { get; set; }
    public string Description { get; set; }
    public string FinalDescription { get; set; }

    // 공격속도 증가율
    public float AttackSpeedIncreasedRate { get; set; }
    // 소형화살 공격력 계수 (최종증강)
    public float SmallArrowAttackRatio { get; set; }
    
    // 마법사 공격속도 증가율 
    public float MageAttackSpeedIncreased { get; set; }
    
    // 추가 화살 공격력 계수 
    public float AdditionalArrowAttackRatio { get; set; }
    // 관통 횟수 증가
    public int PenetrationIncreased { get; set; }
    // 목표 대상 추가 피해 계수
    public float TargetAdditionalDamageRatio  { get; set; }
    public string FinalUpgrade { get; set; }
}

public class WizardAugmentTSV
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int OptionID { get; set; }
    public int Level { get; set; }
    public string Description { get; set; }
    public string FinalDescription { get; set; }
    
    public int Increased_Projectiles { get; set; } //투사체
    public float AttackRatio_ReductionRate  { get; set; } //투사체
    public float OverlapDamage_Reduction  { get; set; } //투사체
    public float Incresed_ExplosiveRange { get; set; } //폭발범위
    public float Dot_HP_Ratio_Duration  { get; set; } //폭발범위
    public float Dot_HP_Ratio  { get; set; } //폭발범위
    public float Debuff_Duration { get; set; } //디버프
    public float Debuff_Rate { get; set; } //디버프
    public float AdditionalExplosion  { get; set; } //디버프
    public float AdditionalExplosion_Ratio { get; set; } //디버프
}


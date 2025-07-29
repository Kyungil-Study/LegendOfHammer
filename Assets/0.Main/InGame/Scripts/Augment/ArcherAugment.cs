

using UnityEngine;

public static class ArcherAugmentFactory
{
    public static ArcherAugment CreateAugment(ArcherAugmentTSV tsv)
    {
        // todo: OptionID에 따라 다른 증강 클래스를 생성을 인스펙터에서 지원
        switch (tsv.OptionID)
        {
            case 10001:
                return new ArcherSpeedAugment(tsv);
            case 10002:
                return new ArcherProjectileAugment(tsv);
            case 10003:
                return new ArcherPenetrationAugment(tsv);
            default:
                throw new System.ArgumentException($"Unknown Archer Augment OptionID: {tsv.OptionID}");
        }
    }
}

public class ArcherSpeedAugment : ArcherAugment
{
    // 공격속도 증가율
    public float AttackSpeedIncreasedRate { get; set; }
    // 소형화살 공격력 계수 (최종증강)
    public float SmallArrowAttackRatio { get; set; }
    
    // 마법사 공격속도 증가율 
    public float MageAttackSpeedIncreased { get; set; }

    
    public ArcherSpeedAugment(ArcherAugmentTSV tsv) : base(tsv)
    {
        AttackSpeedIncreasedRate = tsv.AttackSpeedIncreasedRate;
        SmallArrowAttackRatio = tsv.SmallArrowAttackRatio;
        MageAttackSpeedIncreased = tsv.MageAttackSpeedIncreased;
    }

    public override void Apply(Archer archer, bool isFinalUpgrade)
    {
        Debug.Log($"Applying Archer Speed Augment: {AttackSpeedIncreasedRate}, Final Upgrade: {isFinalUpgrade}");
        // 공격속도 증가 적용
        archer.BonusAttackSpeed = AttackSpeedIncreasedRate;
        archer.mageAttackSpeedFactor = MageAttackSpeedIncreased;
        archer.BonusAttackFactor = SmallArrowAttackRatio;
        
        archer.IsFinalProjectile = isFinalUpgrade;
        
    }
}

public class ArcherProjectileAugment : ArcherAugment
{
    // 추가 화살 공격력 계수
    public float AdditionalArrowAttackRatio { get; set; }
    

    public ArcherProjectileAugment(ArcherAugmentTSV tsv) : base(tsv)
    {
        AdditionalArrowAttackRatio = tsv.AdditionalArrowAttackRatio;
    }

    public override void Apply(Archer archer, bool isFinalUpgrade)
    {
        Debug.Log($"Applying Archer Projectile Augment: {AdditionalArrowAttackRatio}, Final Upgrade: {isFinalUpgrade}");
        
        archer.IsSubProjectile = true;
        
        archer.subProjectileCount = Level; // 서브 화살 개수 초기화
        
        // 추가 화살 공격력 계수 적용
        archer.subProjectileAttackFactor = AdditionalArrowAttackRatio;
        
        archer.IsFinalSubProjectile = isFinalUpgrade;
    }
}

public class ArcherPenetrationAugment : ArcherAugment
{
    // 관통 횟수 증가
    public int PenetrationCountIncreased { get; set; }
    public float TargetAdditionalDamageRatio  { get; set; }

    public ArcherPenetrationAugment(ArcherAugmentTSV tsv) : base(tsv)
    {
        PenetrationCountIncreased = tsv.PenetrationIncreased;
        TargetAdditionalDamageRatio = tsv.TargetAdditionalDamageRatio;
    }

    public override void Apply(Archer archer, bool isFinalUpgrade)
    {
        Debug.Log($" Applying Archer Penetration Augment: {PenetrationCountIncreased}, Final Upgrade: {isFinalUpgrade}");
        // 관통 횟수 증가 적용
        archer.pierceLimit += PenetrationCountIncreased;
        archer.targetAdditionalDamageFactor = TargetAdditionalDamageRatio;
        if (isFinalUpgrade)
        {
            archer.pierceLimit = 1000000; // 최종 업그레이드 시 관통 횟수 무제한  
        }
    }
}

public abstract class ArcherAugment : ClassAugment
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int OptionID { get; set; }

    public int Level { get; set; }
    public string Description { get; set; }
    public string FinalDescription { get; set; }

    public sealed override void Apply(Hero hero, bool isFinalUpgrade)
    {
        Apply(hero as Archer, isFinalUpgrade);
    }
    public abstract void Apply(Archer archer, bool isFinalUpgrade);

    public ArcherAugment(ArcherAugmentTSV tsv)
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
        return AugmentType.Archer;
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



public static class WarriorAugmentFactory
{
    public static WarriorAugment CreateAugment(WarriorAugmentTSV tsv)
    {
        // todo: OptionID에 따라 다른 증강 클래스를 생성하는 로직을 인스펙터에서 지원 
        switch (tsv.OptionID)
        {
            case 8001:
                return new WarriorDashCoolAugment(tsv);
            case 8002:
                return new WarriorKnockbackAugment(tsv);
            case 8003:
                return new WarriorArcWaveAugment(tsv);
            default:
                throw new System.ArgumentException($"Unknown OptionID: {tsv.OptionID}");
        }
    }
}

public class WarriorDashCoolAugment : WarriorAugment
{
    // 대시 쿨타임 감소
    public int DashCoolDown { get; set; }
    public float ExcuteHpRatio { get; set; }

    // 넉백 보너스
    
    public WarriorDashCoolAugment(WarriorAugmentTSV tsv) : base(tsv)
    {
        DashCoolDown = tsv.DashCoolDown;
        ExcuteHpRatio = tsv.ExcuteHpRatio;
    }

    public override void Apply(Warrior warrior, bool isFinalUpgrade)
    {
    }
}

public class WarriorKnockbackAugment : WarriorAugment
{
    // 넉백 보너스
    public int KnockbackBonus { get; set; }
    public float CollisionHpRatio { get; set; }
    
    public WarriorKnockbackAugment(WarriorAugmentTSV tsv) : base(tsv)
    {
        KnockbackBonus = tsv.KnockbackBonus;
        CollisionHpRatio = tsv.CollisionHpRatio;
    }

    public override void Apply(Warrior warrior, bool isFinalUpgrade)
    {
    }
}

public class WarriorArcWaveAugment : WarriorAugment
{
    // 아크웨이브 크기
    public int ArcWaveSize { get; set; }
    // 아크웨이브 공격력 계수
    public float ArcWaveAttackPowerRatio { get; set; }
    public int DashCharge { get; set; }

    
    public WarriorArcWaveAugment(WarriorAugmentTSV tsv) : base(tsv)
    {
        ArcWaveSize = tsv.ArcWaveSize;
        ArcWaveAttackPowerRatio = tsv.ArcWaveAttackPowerRatio;
    }

    public override void Apply(Warrior warrior, bool isFinalUpgrade)
    {
    }
}


public abstract class WarriorAugment : ClassAugment
{
    
    public int ID { get; set; }
    public string Name { get; set; }
    public int OptionID { get; set; }
    public int Level { get; set; }
    public string Description { get; set; }
    public string FinalDescription { get; set; }
    
    public WarriorAugment(WarriorAugmentTSV tsv) : base()
    {
        ID = tsv.ID;
        Name = tsv.Name;
        OptionID = tsv.OptionID;
        Level = tsv.Level;
        Description = tsv.Description;
        FinalDescription = tsv.FinalDescription;
    }
    
    public sealed override void Apply(Hero hero, bool isFinalUpgrade)
    {
        Apply(hero as Warrior, isFinalUpgrade);
    }
    
    public abstract void Apply(Warrior warrior, bool isFinalUpgrade);
    
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
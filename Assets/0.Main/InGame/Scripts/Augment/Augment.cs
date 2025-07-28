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
    
    public bool IsCommon() 
    {
        return GetAugmentType() == AugmentType.Common;
    }

    public abstract Sprite GetIcon();
}

public abstract class ClassAugment : Augment
{
    public abstract int GetLevel();
    
    public abstract void Apply(Hero hero, bool isFinalUpgrade);
    
    public sealed override Sprite GetIcon()
    {
        return ClassAugmentManager.Instance.GetIcon(GetID());
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Hero : MonoBehaviour
{
    [SerializeField] protected Squad squad;
    [ShowInInspector] protected Squad.SquadStats squadStats => squad.stats;

    
    [LabelText("기본 공격력"), SerializeField] private int baseAttackDamage;
    public int BaseAttackDamage
    {
        get => baseAttackDamage;
        set => baseAttackDamage = value;
    }
    [ShowInInspector] public int HeroAttackDamage
    {
        get
        {
            int baseAttackBonus = (int)(baseAttackDamage * squad.stats.AttackDamageFactor);
            return baseAttackDamage + baseAttackBonus;
        }
    }
    
    [LabelText("초당 공격 횟수"),SerializeField] private float attackPerSec;
    [ShowInInspector] public float HeroAttackPerSec
    {
        get
        {
            var heroAttackPerSec = attackPerSec * (1 - squad.stats.DecreaseAttackSpeed);
            return Mathf.Max(heroAttackPerSec, 0.001f) ;
        }
       
    }
    [ShowInInspector] protected virtual float AttackCooldown => 1 / HeroAttackPerSec;
    protected float leftCooldown;
    protected bool bAutoFire = true;
    
   

    protected virtual void Attack() { }

    public abstract int CalculateDamage(bool isCritical = false);

    public virtual int CalculateDamage(BaseDamageCalcArgs args)
    {
        return CalculateDamage(args.IsCritical);
    }

    protected virtual void Awake()
    {
    }

    protected virtual void Update()
    {
        leftCooldown -= Time.deltaTime;
        if (bAutoFire && leftCooldown <= 0)
        {
            Attack();
            ApplyCooldown();
        }
    }
    
    protected virtual void ApplyCooldown()
    {
        leftCooldown = AttackCooldown;
    }
}

public interface IDamageCalcArgs { }

public class BaseDamageCalcArgs : IDamageCalcArgs
{
    public bool IsCritical { get; set; } = false;

    public BaseDamageCalcArgs(bool isCritical = false)
    {
        IsCritical = isCritical;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Hero : MonoBehaviour
{
    protected Squad squad;
    protected Squad.SquadStats squadStats;
    public int baseAttackDamage;
    public float attackPerSec;
    protected virtual float AttackCooldown => 1 / attackPerSec;
    protected float leftCooldown;
    protected bool bAutoFire = true;
    
    // 위자드 디버프
    public float DebuffDuration;
    public float DebuffRate;

    protected virtual void Attack() { }
    public abstract int CalculateDamage(bool isCritical = false);

    protected virtual void Awake()
    {
        squad = Squad.Instance;
        squadStats = Squad.Instance.stats;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Hero : MonoBehaviour
{
    protected Squad squad;
    protected Squad.SquadStats baseStats;
    public int baseAttackDamage;
    public float attackPerSec;
    protected float AttackCooldown => CalculateCooldown();
    protected float leftCooldown;
    protected bool bAutoFire = true;
    public int Damage => CalculateDamage();

    protected virtual void Attack() { }
    protected abstract int CalculateDamage(bool isCritical = false);

    private void Awake()
    {
        squad = Squad.Instance;
        baseStats = Squad.Instance.stats;
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
    
    protected virtual float CalculateCooldown()
    {
        return 1 / attackPerSec;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hero : MonoBehaviour
{
    protected Squad.SquadStats baseStats;
    public int attackDamage;
    public float attackPerSec;
    protected float attackCooldown;
    protected bool bAutoFire = true;

    protected virtual void Attack() { }
    protected abstract int CalculateDamage(bool isCritical = false);

    private void Awake()
    {
        baseStats = Squad.Instance.stats;
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime;
        if (bAutoFire && attackCooldown <= 0)
        {
            Attack();
            ApplyCooldown();
        }
    }
    
    protected void ApplyCooldown()
    {
        attackCooldown = 1 / attackPerSec;
    }
}

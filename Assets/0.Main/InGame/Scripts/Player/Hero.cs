using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hero : MonoBehaviour
{
    protected Squad squad;
    protected Squad.SquadStats baseStats;
    public int attackDamage;
    public float attackPerSec;
    protected float attackCooldown;
    protected bool bAutoFire = true;
    public int Damage => CalculateDamage();

    protected virtual void Attack() { }
    protected abstract int CalculateDamage(bool isCritical = false);

    private void Awake()
    {
        squad = Squad.Instance;
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

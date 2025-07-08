using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hero : MonoBehaviour
{
    protected Squad.SquadStats baseStats;
    public int attackDamage;
    public float attackPerSec;
    private float m_AttackCooldown;
    protected bool bAutoFire = true;

    protected virtual void Attack() { }
    protected abstract int CalculateDamage(bool isCritical = false);

    private void Awake()
    {
        baseStats = Squad.Instance.stats;
    }

    private void Update()
    {
        m_AttackCooldown -= Time.deltaTime;
        if (bAutoFire && m_AttackCooldown <= 0)
        {
            Attack();
            ApplyCooldown();
        }
    }
    
    protected void ApplyCooldown()
    {
        m_AttackCooldown = 1 / attackPerSec;
    }
}

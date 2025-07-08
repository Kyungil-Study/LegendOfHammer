using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hero : MonoBehaviour
{
    public int attackDamage;
    public float attackPerSec;
    private float m_AttackCooldown;
    protected bool bAutoFire = true;
    
    protected abstract void Attack();
    protected abstract int CalculateDamage();
    
    private void Update()
    {
        m_AttackCooldown -= Time.deltaTime;
        if (bAutoFire && m_AttackCooldown <= 0)
        {
            Attack();
            ApplyCooldown();
        }
    }
    
    private void ApplyCooldown()
    {
        m_AttackCooldown = 1 / attackPerSec;
    }
}

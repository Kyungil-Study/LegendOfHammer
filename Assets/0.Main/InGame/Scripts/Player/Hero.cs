using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hero : MonoBehaviour
{
    public int attackDamage;
    public float attackPerSec;
    public float attackCooldown;
    public abstract void Attack();

    protected void ApplyCooldown()
    {
        attackCooldown = 1 / attackPerSec;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class HeroProjectile : MonoBehaviour
{
    public Action OnHit { get; set; }
    
    public Hero Owner { get; set; }
    private bool mb_IsFired = false;
    public bool IsCritical { get; set; } = false;
    public virtual int Damage => Owner.CalculateDamage(new BaseDamageCalcArgs(IsCritical));
    public Func<GameObject> FindTargetFunc { get; set; }
    private Vector3 m_TargetDirection;
    [Range(0,10)] public float speed;
    private float m_LifeTime = 5f;

    private void Update()
    {
        if (mb_IsFired)
        {
            transform.Translate(m_TargetDirection * (speed * Distance.STANDARD_DISTANCE * Time.deltaTime), Space.World);
        }
        
        m_LifeTime -= Time.deltaTime;
        if (m_LifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Fire()
    {
        SetDirection();
        mb_IsFired = true;
    }

    protected virtual void SetDirection()
    {
        GameObject target = FindTargetFunc != null ? FindTargetFunc() : FindTarget();
        
        if (target != null)
        {
            m_TargetDirection = target.transform.position - transform.position;
            m_TargetDirection.Normalize();
        }
        else
        {
            m_TargetDirection = Vector3.up;
        }
        transform.up = m_TargetDirection;
    }

    protected virtual GameObject FindTarget()
    {
        return FindClosestEnemy();
    }
    
    protected GameObject FindClosestEnemy()
    {
        // return BattleManager.Instance.GetAllMonsters().OrderByDescending(monster =>
        // {
        //     return Vector3.Distance(transform.position, monster.transform.position);
        // }).First().gameObject;
        
        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;
        foreach (var monster in BattleManager.Instance.GetAllMonsters())
        {
            float distance = Vector3.Distance(transform.position, monster.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = monster.gameObject;
            }
        }

        return closestEnemy;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (BattleManager.TryGetMonsterBy(other, out Monster monster))
        {
            Hit(monster);
        }
    }

    protected abstract void Hit(Monster target);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroProjectile : MonoBehaviour
{
    private bool mb_IsFired = false;
    public int damage;
    private Vector3 m_TargetDirection;
    private float m_Speed;
    [Range(0, 10)] public float speed;
    private float m_LifeTime = 5f;

    private void Awake()
    {
        SetSpeed();
    }

    private void Update()
    {
        if (mb_IsFired)
        {
            transform.Translate(m_TargetDirection * (m_Speed * Time.deltaTime));
        }
        
        m_LifeTime -= Time.deltaTime;
        if (m_LifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    private void SetSpeed()
    {
        m_Speed = Squad.STANDARD_DISTANCE * speed;
    }
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        m_Speed = Squad.STANDARD_DISTANCE * speed;
    }

    public void Fire()
    {
        SetDirection();
        mb_IsFired = true;
    }

    protected virtual void SetDirection()
    {
            m_TargetDirection = Vector3.up;
            return;
            // TODO: Remove test code
            #pragma warning disable CS0162
        var target = FindTarget();
        if (target != null)
        {
            m_TargetDirection = target.transform.position - transform.position;
            m_TargetDirection.Normalize();
        }
        else
        {
            m_TargetDirection = Vector3.up;
        }
            #pragma warning restore CS0162
    }

    protected virtual GameObject FindTarget()
    {
        return FindClosestEnemy();
    }
    
    protected GameObject FindClosestEnemy()
    {
        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var monster in BattleManager.GetAllMonsters())
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
        if (BattleManager.GetMonsterBy(other, out Monster monster))
        {
            Hit(monster);
        }
    }

    protected abstract void Hit(Monster target);
}

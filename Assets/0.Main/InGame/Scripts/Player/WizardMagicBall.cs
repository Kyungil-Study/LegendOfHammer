using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardMagicBall : MonoBehaviour
{
    private bool mb_IsFired = false;
    [HideInInspector] public int damage = 10;
    private Vector3 m_TargetDirection;
    [Range(0, 10)] public float speed = 1f;
    private float m_LifeTime = 5f;
    
    public float explosionRadius = 1f;

    private void Update()
    {
        m_LifeTime -= Time.deltaTime;
        if (m_LifeTime <= 0)
        {
            Destroy(gameObject);
        }
        
        if (mb_IsFired)
        {
            transform.Translate(m_TargetDirection * (speed * Time.deltaTime));
        }
    }

    public void Fire()
    {
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

        mb_IsFired = true;
    }

    // TODO: Implement target finding logic and return the target Monster.
    private GameObject FindTarget()
    {
        return null;
    }

    private void OnHit()
    {
        IBattleCharacter[] enemies = FindTargetsInRadius();
        foreach (var enemy in enemies)
        {
            TakeDamageEventArgs eventArgs = new TakeDamageEventArgs(
                Squad.Instance,
                enemy, 
                damage
            );
            BattleEventManager.Instance.CallEvent(eventArgs);
        }
        // TODO: Play explosion effect here
        Destroy(gameObject);
    }

    // TODO: Implement target finding logic
    private IBattleCharacter[] FindTargetsInRadius()
    {
        return null;
    }
}

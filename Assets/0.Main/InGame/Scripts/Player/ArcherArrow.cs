using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine;

public class ArcherArrow : MonoBehaviour
{
    private bool mb_IsFired = false;
    public int damage = 10;
    private Vector3 m_TargetDirection;
    private float m_Speed = 1f;
    private float m_LifeTime = 5f;

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

    private GameObject FindTarget()
    {
        // TODO: Implement target finding logic and return the target Monster.
        return null;
    }

    private void OnHit()
    {
        TakeDamageEventArgs eventArgs = new TakeDamageEventArgs(
            Squad.Instance,
            FindTarget().GetComponent<IBattleCharacter>(), 
            damage
        );
        BattleEventManager.Instance.CallEvent(eventArgs);
        Destroy(gameObject);
    }
}

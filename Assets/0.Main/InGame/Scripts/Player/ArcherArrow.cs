using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherArrow : MonoBehaviour
{
    private bool mb_IsFired = false;
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
    }

    private GameObject FindTarget()
    {
        // TODO: Implement target finding logic and return the target Monster.
        return null;
    }
}

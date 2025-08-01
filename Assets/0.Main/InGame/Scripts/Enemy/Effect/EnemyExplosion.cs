using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    private float radius;
    private int   damage;
    private LayerMask targetMask;
    private IBattleCharacter attacker;

    [SerializeField] private float  lifetime = 1f;

    public void Initialize(IBattleCharacter attacker, int damage, float radius, LayerMask mask)
    {
        this.attacker   = attacker;
        this.damage     = damage;
        this.radius     = radius;
        this.targetMask = mask;
    }

    void Start()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);
        
        foreach (var col in hits)
        {
            var target = col.GetComponent<IBattleCharacter>();
            
            if (target != null)
            {
                BattleEventManager.CallEvent
                (
                    new TakeDamageEventArgs(attacker, target, DamageType.Enemy, damage)
                );
            }
        }

        StartCoroutine(DestroyAfter());
    }

    private IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

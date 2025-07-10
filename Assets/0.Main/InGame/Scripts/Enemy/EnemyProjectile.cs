using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float   speed;
    private int     damage;
    private LayerMask targetMask;
    private IBattleCharacter attacker;

    public void Initialize(Vector2 dir, float speed, int damage, LayerMask mask, IBattleCharacter attacker)
    {
        this.direction = dir.normalized;
        this.speed     = speed;
        this.damage    = damage;
        this.targetMask= mask;
        this.attacker  = attacker;
    }

    void Update()
    {
        transform.position += (Vector3)direction * (speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        int bit = 1 << col.gameObject.layer;
        if ((targetMask.value & bit) == 0) return;

        if (col.TryGetComponent<IBattleCharacter>(out var target))
        {
            BattleEventManager.Instance.CallEvent(
                new TakeDamageEventArgs(attacker, target, damage)
            );
        }
        Destroy(gameObject);
    }
}
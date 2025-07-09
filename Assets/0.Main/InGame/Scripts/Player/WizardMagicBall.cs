using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardMagicBall : HeroProjectile
{
    private static readonly int EXPLOSION = Animator.StringToHash("Explosion");
    public float explosionRadius = 1f;
    
    protected override void Hit(IBattleCharacter target)
    {
        Explode(transform.position, explosionRadius);
    }
    
    private void Explode(Vector3 position, float radius)
    {
        GetComponentInChildren<Animator>().SetTrigger(EXPLOSION);
        List<Monster> enemies = BattleManager.GetAllEnemyInRadius(position, radius);
        foreach (var enemy in enemies)
        {
            TakeDamageEventArgs eventArgs = new TakeDamageEventArgs(
                Squad.Instance,
                enemy, 
                damage
            );
            BattleEventManager.Instance.CallEvent(eventArgs);
        }
        Destroy(gameObject);
    }
}

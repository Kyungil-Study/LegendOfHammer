using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardMagicBall : HeroProjectile
{
    public GameObject explosionEffectPrefab;
    public float explosionRadius = 1f;
    
    protected override void Hit(Monster target)
    {
        Explode(target.transform.position, explosionRadius);
    }
    
    private void Explode(Vector3 position, float radius)
    {
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
        Destroy(Instantiate(explosionEffectPrefab, position, Quaternion.identity),2f);
        Destroy(gameObject);
    }
}

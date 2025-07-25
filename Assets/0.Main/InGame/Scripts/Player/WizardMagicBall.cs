using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardMagicBall : HeroProjectile
{
    public GameObject explosionEffectPrefab;
    public float explosionRadius = 0.5f;
    
    protected override void Hit(Monster target)
    {
        Explode(target.transform.position, explosionRadius * Distance.STANDARD_DISTANCE);
    }
    
    private void Explode(Vector3 position, float radius)
    {
        DebugDrawUtil.DrawCircle(position, radius, Color.red);
        List<Monster> enemies = BattleManager.GetAllEnemyInRadius(position, radius);
        foreach (var enemy in enemies)
        {
            TakeDamageEventArgs eventArgs = new TakeDamageEventArgs(
                Squad.Instance,
                enemy, 
                Damage
            );
            BattleEventManager.Instance.CallEvent(eventArgs);
            enemy.Stat.AddModifier(new DamageAmpModifier(Owner.DebuffRate, Owner.DebuffDuration));
            Debug.Log($"디버프 수치: {Owner.DebuffRate}, 디버프 시간: {Owner.DebuffDuration}");
        }

        var explosionEffect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
        SetExplosionEffectSize(explosionEffect, radius);
        Destroy(explosionEffect,2f);
        Destroy(gameObject);
    }
    
    private void SetExplosionEffectSize(GameObject effect, float radius)
    {
        float targetSize = radius * Distance.STANDARD_DISTANCE;
        var spriteRenderer = effect.GetComponent<SpriteRenderer>();
        float currentSize = spriteRenderer.sprite.rect.size.x / spriteRenderer.sprite.pixelsPerUnit;
        float scaleFactor = targetSize / currentSize;
        //Debug.Log($"{targetSize} / {currentSize} = {scaleFactor}");
        effect.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }
}
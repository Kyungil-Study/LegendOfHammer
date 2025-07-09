using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Wizard : Hero
{
    public Transform projectileSpawnPoint;
    public WizardMagicBall projectilePrefab;

    protected override void Attack()
    {
        WizardMagicBall projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        projectile.damage = Damage;
        projectile.Fire();
    }

    // TODO: 중복 공격 시 피해 감소율
    // 마법사 마법 구체 폭발 피해량
    // [[{(마법사 기본 공격 피해량 x 치명타 피해량) + 타격 당 데미지} x 받는 피해 증가] x 최종 데미지 증가] x 중복 공격 시 피해 감소율

    protected override int CalculateDamage(bool isCritical = false)
    {
        float critFactor = isCritical ? baseStats.CriticalDamage : 1f;
        return (int)(((attackDamage * critFactor) + baseStats.BonusDamagePerHit) * baseStats.FinalDamageFactor);
    }
}

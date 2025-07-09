using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Archer : Hero
{
    public Transform projectileSpawnPoint;
    public ArcherArrow projectilePrefab;
    
    protected override void Attack()
    {
        ArcherArrow projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        projectile.damage = CalculateDamage();
        projectile.Fire();
    }

    // 궁수 기본 화살 피해량
    // [{(궁수 기본 공격 피해량 x 치명타 피해량) + 타격 당 데미지 + (궁수 기본 공격력 x 표적 대상 추가 피해 계수)} x 받는 피해량 증가] x 최종 데미지 증가
    // 궁수 추가 투사체 피해량 (증강 최종 강화 X)
    // [{(궁수 공격력 x 추가 투세차 공격력 계수) + 타격 당 데미지} x 받는 피해량 증가] x 최종 데미지 증가
    
    protected override int CalculateDamage(bool isCritical = false)
    {
        float critFactor = isCritical ? baseStats.CriticalDamage : 1f;
        return (int)(((attackDamage * critFactor) + baseStats.BonusDamagePerHit + (attackDamage * GetArcheryBonusEffectFactor()) * baseStats.FinalDamageFactor));
    }

    // TODO: 증강에 의한 추가 계수
    private float GetArcheryBonusEffectFactor()
    {
        return 1;
    }
}

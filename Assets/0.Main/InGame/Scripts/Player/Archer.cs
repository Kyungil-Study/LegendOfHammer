using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Archer : Hero
{
    public float BonusAttackSpeed { get; set; }
    protected override float AttackCooldown => 1 / (attackPerSec * BonusAttackSpeed);
    public int pierceLimit = 0;

    public Transform projectileSpawnPoint;
    public ArcherArrow projectilePrefab;
    
    protected override void Attack()
    {
        ArcherArrow projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        projectile.pierceLimit = pierceLimit;
        projectile.IsCritical = Random.Range(0f,1f) <= squadStats.CriticalChance;
        projectile.Fire();
    }

    // 궁수 기본 화살 피해량
    // [{(궁수 기본 공격 피해량 x 치명타 피해량) + 타격 당 데미지 + (궁수 기본 공격력 x 표적 대상 추가 피해 계수)} x 받는 피해량 증가] x 최종 데미지 증가
    public override int CalculateDamage(bool isCritical = false)
    {
        float critFactor = isCritical ? squadStats.CriticalDamage : 1f;
        return (int)(((baseAttackDamage * critFactor) + squadStats.BonusDamagePerHit + (baseAttackDamage * GetArcheryBonusEffectFactor()) * squadStats.FinalDamageFactor));
    }
    
    // TODO: 관통 증강 최종 효과에 따른 대상 추가 피해 계산
    public override int CalculateDamage(params object[] prms)
    {
        return base.CalculateDamage(prms);
    }

    // TODO: 증강에 의한 추가 계수
    private float GetArcheryBonusEffectFactor()
    {
        return 0;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Archer : Hero
{
    [field:SerializeField] public float BonusAttackSpeed { get; set; }
    protected override float AttackCooldown => 1 / (attackPerSec * (1 + BonusAttackSpeed));
    public int pierceLimit = 0; // 관통횟수
    public int projectileCountPerAttack = 1; // 공격당 화살 개수

    // 법사 공속 버프용 계수
    public float mageAttackSpeedFactor = 0f; // 마법사 화살의 공격 속도 계수
    
    public float subProjectileAttackFactor = 1f; // 서브 화살의 공격력 계수
    public int targetAdditionalDamageFactor = 0; // 표적 대상 추가 피해 계수
    
    public Transform projectileSpawnPoint;
    public ArcherArrow projectilePrefab;
    
    protected override void Attack()
    {
        ArcherArrow projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        projectile.Owner = this;
        projectile.pierceLimit = pierceLimit;
        projectile.IsCritical = Random.Range(0f,1f) <= squadStats.CriticalChance;
        projectile.Fire();
    }

    // 궁수 기본 화살 피해량
    // [{(궁수 기본 공격 피해량 x 치명타 피해량) + 타격 당 데미지 + (궁수 기본 공격력 x 표적 대상 추가 피해 계수)} x 받는 피해량 증가] x 최종 데미지 증가
    // TODO: 증강이 있는 경우에 대해서 구현해야함
    public override int CalculateDamage(bool isCritical = false)
    {
        float critFactor = isCritical ? squadStats.CriticalDamage : 1f;
        return (int)(((baseAttackDamage * critFactor) + squadStats.BonusDamagePerHit + (baseAttackDamage * GetArcheryBonusEffectFactor()) * squadStats.FinalDamageFactor));
    }

    // TODO: 증강에 의한 추가 계수
    private float GetArcheryBonusEffectFactor()
    {
        return 0;
    }
}
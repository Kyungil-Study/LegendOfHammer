using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Wizard : Hero
{
    [field:SerializeField] public float BonusAttackSpeed { get; set; }
    protected override float AttackCooldown => 1 / (attackPerSec * (1 + BonusAttackSpeed));
    
    public Transform projectileSpawnPoint;
    public WizardMagicBall projectilePrefab;
    [field:SerializeField]public float ExplosionRadius { get; set; } = 0.5f;

    public int AttackCount = 1;
    public float CurrentExplosionRadius;
    
    public bool FinalDebuff; //디버프 4레벨 여부(죽으면 폭발)
    public bool FinalExplosive; //범위 4레벨 여부(도트딜)
    
    

    protected override void Awake()
    {
        base.Awake();
        var callbacks = BattleEventManager.Instance.Callbacks;
        callbacks.OnStartBattle += OnStartBattle;
        CurrentExplosionRadius = ExplosionRadius;
    }

    private void OnStartBattle(StartBattleEventArgs obj)
    {
        AugmentInventory.Instance.ApplyAugmentsToWizard(this);
    }

    protected override void Attack()
    {
        for (int i = 0; i < AttackCount; i++)
        {
            WizardMagicBall projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            projectile.Owner = this;
            projectile.explosionRadius = CurrentExplosionRadius;
            projectile.IsCritical = Random.Range(0f,1f) <= squadStats.CriticalChance;
            projectile.Fire();
        }
        Debug.Log($"공격 개수: {AttackCount}, 공격 범위: {CurrentExplosionRadius}");
    }

    // TODO: 중복 공격 시 피해 감소율
    // 마법사 마법 구체 폭발 피해량
    // [[{(마법사 기본 공격 피해량 x 치명타 피해량) + 타격 당 데미지} x 받는 피해 증가] x 최종 데미지 증가] x 중복 공격 시 피해 감소율
    public override int CalculateDamage(bool isCritical = false)
    {
        float critFactor = isCritical ? squadStats.CriticalDamage : 1f;
        return (int)(((baseAttackDamage * critFactor) + squadStats.BonusDamagePerHit) * squadStats.FinalDamageFactor);
    }
}

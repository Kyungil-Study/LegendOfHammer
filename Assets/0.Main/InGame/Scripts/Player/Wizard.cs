using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public enum wizardAttackType
{
    Normal,
    Explosive,
    Debuff
}

public class Wizard : Hero
{
    [field:SerializeField] public float BonusAttackSpeed { get; set; }
    protected override float AttackCooldown => 1 / (HeroAttackPerSec * (1 + BonusAttackSpeed));
    
    public Transform projectileSpawnPoint;
    public WizardMagicBall projectilePrefab;
    [field:SerializeField]public float ExplosionRadius { get; set; } = 0.5f;

    public int AttackCount = 1;
    public float CurrentExplosionRadius;
    
    public bool FinalDebuff; //디버프 4레벨 여부(죽으면 폭발)
    public bool FinalExplosive; //범위 4레벨 여부(도트딜)
    
    [SerializeField] private GameObject deathExplosionEffectPrefab;
    [SerializeField] private float deathExplosionRadius = 1f; // 사망 폭발 반경 (월드 단위)
    
    protected override void Awake()
    {
        base.Awake();
        BattleEventManager.RegistEvent<StartBattleEventArgs>(OnStartBattle);
        BattleEventManager.RegistEvent<DeathEventArgs>(OnAnyDeath);
        CurrentExplosionRadius = ExplosionRadius;
    }

    private void OnStartBattle(StartBattleEventArgs obj)
    {
        AugmentInventory.Instance.ApplyAugmentsToWizard(this);
    }
    
    protected override void Attack()
    {
        if (AttackCount == 5)
        {
            float delayBetweenWaves = 0.1f;
            int shotsPerWave = 5;
            int totalWaves = 2;

            StartCoroutine(FireSpreadShots(shotsPerWave, totalWaves, delayBetweenWaves));
        }
        else
        {
            FireGuidedProjectiles();
        }
    }

    // TODO: 중복 공격 시 피해 감소율
    // 마법사 마법 구체 폭발 피해량
    // [[{(마법사 기본 공격 피해량 x 치명타 피해량) + 타격 당 데미지} x 받는 피해 증가] x 최종 데미지 증가] x 중복 공격 시 피해 감소율
    public override int CalculateDamage(bool isCritical = false)
    {
        float critFactor = isCritical ? squadStats.CriticalDamage : 1f;
        return Mathf.RoundToInt(((HeroAttackDamage * critFactor) + squadStats.BonusDamagePerHit) * squadStats.FinalDamageFactor);
    }
    
    // 몬스터가 죽었을 때 폭발 처리
    private void OnAnyDeath(DeathEventArgs args)
    {
        if (FinalDebuff == false) return; // 최종 디버프 미보유 시 무시

        // 몬스터만 처리
        if (args.Target is not Monster monster) return;

        // 디버프가 적용되어 있어야 폭발
        if (monster.Stat.HasModifier<DamageAmpModifier>() == false) return;

        Vector3 pos = monster.transform.position;
        float radius = deathExplosionRadius * Distance.STANDARD_DISTANCE;

        DebugDrawUtil.DrawCircle(pos, radius, Color.green);

        // 주변 적 탐지
        List<Monster> enemies = BattleManager.GetAllEnemyInRadius(pos, radius);

        var crit = Random.Range(0f,1f) <= squadStats.CriticalChance;
        foreach (var enemy in enemies)
        {
            var eventArgs = new TakeDamageEventArgs(
                                Squad.Instance, enemy, crit ? DamageType.Critical : DamageType.Wizard,
                CalculateDamage(crit) // 기본 공격력 기반 피해
            );
            //BattleEventManager.Instance.CallEvent(eventArgs);
            //enemy.Stat.AddModifier(new DamageAmpModifier(DebuffRate, DebuffDuration)); // 디버프 재적용
        }

        // 폭발 이펙트
        if (deathExplosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathExplosionEffectPrefab, pos, Quaternion.identity);
            SetExplosionEffectSize(effect, radius);
            Destroy(effect, 2f);
        }
        
        Debug.Log("폭발은 예술이다");
    }

    // 이펙트 크기 조절
    private void SetExplosionEffectSize(GameObject effect, float radius)
    {
        float targetSize = radius;
        var spriteRenderer = effect.GetComponent<SpriteRenderer>();
        float currentSize = spriteRenderer.sprite.rect.size.x / spriteRenderer.sprite.pixelsPerUnit;
        float scaleFactor = targetSize / currentSize;
        effect.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }
    
    private void FireGuidedProjectiles()
    {
        var allMonsters = BattleManager.Instance.GetAllMonsters()
            .OrderBy(m => Vector3.Distance(transform.position, m.transform.position))
            .Take(AttackCount)
            .ToList();

        for (int i = 0; i < AttackCount; i++)
        {
            WizardMagicBall projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            projectile.Owner = this;
            projectile.explosionRadius = CurrentExplosionRadius;
            projectile.IsCritical = Random.Range(0f, 1f) <= squadStats.CriticalChance;

            if (i < allMonsters.Count)
            {
                GameObject target = allMonsters[i].gameObject;
                projectile.FindTargetFunc = () => target;
            }

            projectile.Fire();
        }
    }
    
    private IEnumerator FireSpreadShots(int shotsPerWave, int waveCount, float delay)
    {
        float totalAngle = 80f;

        for (int wave = 0; wave < waveCount; wave++)
        {
            float angleStep = shotsPerWave > 1 ? totalAngle / (shotsPerWave - 1) : 0f;
            float startAngle = -totalAngle / 2f;

            for (int i = 0; i < shotsPerWave; i++)
            {
                float angle = startAngle + angleStep * i;
                Vector3 baseDirection = projectileSpawnPoint.up;
                Vector3 rotatedDirection = Quaternion.Euler(0f, 0f, angle) * baseDirection;

                WizardMagicBall projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
                projectile.Owner = this;
                projectile.explosionRadius = CurrentExplosionRadius;
                projectile.IsCritical = Random.Range(0f, 1f) <= squadStats.CriticalChance;

                projectile.FindTargetFunc = null; // 유도 비활성화
                projectile.FireWithDirection(rotatedDirection);
            }

            yield return new WaitForSeconds(delay);
        }
    }
}

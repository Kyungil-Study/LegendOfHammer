using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Archer : Hero
{
    [field:SerializeField] public float BonusAttackSpeed { get; set; }
    protected override float AttackCooldown => 1 / (attackPerSec * (1 + BonusAttackSpeed));
    
    

    public float BonusAttackFactor = 1; // 추가 화살 공격력 계수
    // 법사 공속 버프용 계수
    public float mageAttackSpeedFactor = 0f; // 마법사 화살의 공격 속도 계수
    
    public float subProjectileAttackFactor = 0; // 서브 화살의 공격력 계수
    public float targetAdditionalDamageFactor = 0; // 표적 대상 추가 피해 계수
    
    public bool IsFinalPenetration { get; set; } = false; // 최종 관통 여부
    public int pierceLimit = 0; // 관통횟수
    
    [Header("Projectile Settings")]
    
    [Space(10),Header("Noraml Projectile")]
    public Transform NormalProjectileSpawnPoint;
    public ArcherArrow NormalProjectilePrefab;
    [Space(10),Header("Final Projectile")]
    public Transform[] finalProjectileSpawnPoints; // 최종 화살 발사 위치들
    public ArcherArrow finalProjectilePrefab;
    public bool IsFinalProjectile { get; set; } = false; // 최종 화살 여부
    
    [Header("Sub Projectile Settings")]
    [SerializeField] private ArcherArrow subProjectilePrefab; // 서브 화살 프리팹
    public int subProjectileCount { get; set; } = 0; // 서브 화살 개수
    
    public bool IsSubProjectile { get; set; } = false;
    public bool IsFinalSubProjectile { get; set; } = false;
    

    protected override void Awake()
    {
        base.Awake();
        var callbacks = BattleEventManager.Instance.Callbacks;
        callbacks.OnStartBattle += OnStartBattle;
    }

    private void OnStartBattle(StartBattleEventArgs args)
    {
        AugmentInventory.Instance.ApplyAugmentsToArcher(this);
    }


    protected override void Attack()
    {
        BasicAttack();
        SubAttack();
    }
    
    private void BasicAttack()
    {
        var projectileSample = IsFinalProjectile ? finalProjectilePrefab : NormalProjectilePrefab;
        var projectileSpawn = IsFinalProjectile ? finalProjectileSpawnPoints : new Transform[] { NormalProjectileSpawnPoint };

        Debug.Log($" BasicAttack Sample Projectile: {projectileSample.name}, Spawn Points Count: {projectileSpawn.Length}");
        foreach (var spawPoint in projectileSpawn)
        {
            ArcherArrow projectile = Instantiate(projectileSample, spawPoint.position, spawPoint.rotation);
            projectile.Owner = this;
            projectile.pierceLimit = pierceLimit;
            projectile.IsCritical = Random.Range(0f,1f) <= squadStats.CriticalChance;
            projectile.Fire();
        }
    }
    
    private void SubAttack()
    {
        if(IsSubProjectile == false)
        {
            return;
        }

        #region 추가 투사체 조준 알고리즘
        Func<GameObject> findFunc = () => // 타겟 몬스터를 찾는 함수 두번째 가까운 적
        {
            var monsters = BattleManager.Instance.GetAllMonsters();
            if (monsters.Any() == false)
            {
                Debug.Log("No monsters found to target.");
                return null;
            }

            var monstersOrdered = monsters.OrderByDescending(monster =>
                {
                    var id = monster.EnemyID;
                    var data = EnemyDataManager.Instance.Records[id];
                    return data.Enemy_Rank;
                })
                .ThenByDescending(monster => monster.MaxHP)
                .ThenBy(monster => Vector3.Distance(monster.transform.position, transform.position)).ToList();

            if (monstersOrdered.Count() < 2)
            {
                return monstersOrdered.First().gameObject;
            }

            return monstersOrdered[1].gameObject;
        };
        

        #endregion

        #region 스폰 포인트 샘플 결정
        List<Transform> projectileSpawnPoints = new List<Transform>();
        ArcherArrow sample = null;        
        if (IsFinalSubProjectile)
        {
            if (IsFinalProjectile)
            {
                // 최종 화살이 발사된 후에 서브 화살을 발사하는 경우
                projectileSpawnPoints = finalProjectileSpawnPoints.ToList();
                sample = finalProjectilePrefab;
            }
            else
            {
                // 최종 화살이 아닌 경우, 서브 화살 발사 위치를 기본 위치로 설정
                projectileSpawnPoints = Enumerable.Repeat(NormalProjectileSpawnPoint, subProjectileCount).ToList();
                sample = subProjectilePrefab;
            }
        }
        else
        {
            // 서브 화살이 아닌 경우, 기본 화살 발사 위치로 설정
            projectileSpawnPoints = Enumerable.Repeat(NormalProjectileSpawnPoint, subProjectileCount).ToList();
            sample = subProjectilePrefab;
        }
        #endregion
        Debug.Log($"SubAttack Sample Projectile: {sample.name}, Spawn Points Count: {projectileSpawnPoints.Count}");
        #region 투사체 스폰 발사
        foreach (var spawPoint in projectileSpawnPoints)
        {
            ArcherArrow subProjectile = Instantiate(sample, spawPoint.position, spawPoint.rotation);
            subProjectile.Owner = this;
            subProjectile.pierceLimit = IsFinalSubProjectile ? pierceLimit : 0;
            var critical = Random.Range(0f, 1f) <= squadStats.CriticalChance;
            subProjectile.IsCritical = IsFinalSubProjectile ? critical : false;
            subProjectile.FindTargetFunc = findFunc;
            subProjectile.DamageCalculationFunc = CalculateSubProjectileDamage;
            subProjectile.Fire();
        }
        #endregion
        
    }


    // 궁수 기본 화살 피해량
    // [{(궁수 기본 공격 피해량 x 치명타 피해량) + 타격 당 데미지 + (궁수 기본 공격력 x 표적 대상 추가 피해 계수)} x 받는 피해량 증가] x 최종 데미지 증가
    // TODO: 증강이 있는 경우에 대해서 구현해야함
    public override int CalculateDamage(bool isCritical = false)
    {
        float critFactor = isCritical ? squadStats.CriticalDamage : 1f;
        return (int)(((baseAttackDamage * critFactor) + squadStats.BonusDamagePerHit + (baseAttackDamage * GetArcheryBonusEffectFactor()) * squadStats.FinalDamageFactor));
    }
    
    public int CalculateSubProjectileDamage(bool isCritical = false)
    {
        return (int)(CalculateDamage() * subProjectileAttackFactor);
    }

    // TODO: 증강에 의한 추가 계수
    private float GetArcheryBonusEffectFactor()
    {
        return 0;
    }
}
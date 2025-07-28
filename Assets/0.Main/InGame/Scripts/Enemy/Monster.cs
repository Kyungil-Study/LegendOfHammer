using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour, IBattleCharacter
{
    public class MonsterRuntimeState
    {
        public bool  Detected;
        public bool  StoppedFlying;
        public float ShieldRate = 1f;
    }
    public MonsterRuntimeState State { get; } = new MonsterRuntimeState();
    
    private EnemyID enemyID;           
    public EnemyID EnemyID => enemyID;                  
    public void SetEnemyID(EnemyID id) => enemyID = id;
    
    public bool IsTestMode = false;
    
    public void MonsterTest(EnemyMovementPattern movePattern, EnemyAttackPattern attackPattern) 
    {
        move   = MovementFactory.Create(movePattern, this);
        attack = AttackFactory.Create(attackPattern, this);

        move?.Init(this);
        attack?.Init(this);
        attack?.Start();
    }
    
    public GameObject Player => player;
    public void SetPlayer(GameObject player) => this.player = player;
    
    [Header("충돌 레이어 설정")]
    private GameObject player;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private LayerMask projectileLayerMask;
    public LayerMask PlayerLayerMask      => playerLayerMask;
    public LayerMask ProjectileLayerMask  => projectileLayerMask;

    [Header("이동 설정 : 지그재그")] [SerializeField] private ZigzagMoveConfig zigzagCfg = new();
    [Header("이동 설정 : 추적형")]  [SerializeField] private ChaseMoveConfig  chaseCfg  = new();
    [Header("이동 설정 : 체공형")]  [SerializeField] private FlyingMoveConfig flyingCfg = new();
    public ZigzagMoveConfig  ZigzagConfig      => zigzagCfg;
    public ChaseMoveConfig   ChaseConfig       => chaseCfg;
    public FlyingMoveConfig  FlyingMoveConfig  => flyingCfg;

    [Header("공격 설정 : 자폭형")] [SerializeField] private SuicideAttackConfig suicideCfg;
    [Header("공격 설정 : 쉴드형")] [SerializeField] private ShieldConfig shieldCfg;
    [Header("공격 설정 : 저격형")] [SerializeField] private SniperAttackConfig sniperCfg;
    [Header("공격 설정 : 분사형")] [SerializeField] private SpreadAttackConfig spreadCfg;
    [Header("공격 설정 : 방사형")] [SerializeField] private RadialAttackConfig radialCfg;
    [Header("공격 설정 : 체공형")] [SerializeField] private FlyingAttackConfig flyingAtkCfg;
    public SuicideAttackConfig SuicideCfg   => suicideCfg;
    public ShieldConfig       ShieldCfg     => shieldCfg;
    public SniperAttackConfig SniperCfg     => sniperCfg;
    public SpreadAttackConfig SpreadCfg     => spreadCfg;
    public RadialAttackConfig RadialCfg     => radialCfg;
    public FlyingAttackConfig FlyingAtkCfg  => flyingAtkCfg;
    
    [Header("넉백 설정")] [SerializeField] private float knockbackDuration = 0.2f;
    [Header("사망 이펙트")] [SerializeField] private GameObject deathEffectPrefab;
    
    private IAttackBehaviour attack;
    private IMoveBehaviour move;
    
    private Rigidbody2D rigid;
    
    private MonsterStat stat;
    private MonsterScale scale;
    
    public MonsterStat Stat => stat;

    void Awake()
    {
        scale = GetComponent<MonsterScale>();
        stat = GetComponent<MonsterStat>();
        
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        BattleEventManager.RegistEvent<ChargeCollisionArgs>(OnChargeCollision);;
        BattleManager.RegisterMonster(this);
    }

    void OnDisable()
    {
        BattleEventManager.UnregistEvent<ChargeCollisionArgs>(OnChargeCollision);;
        BattleManager.UnregisterMonster(this);
    }
    
    void Start()
    {
        if (IsTestMode) return;
        
        var data = EnemyDataManager.Instance.Records[enemyID];
        var stage = BattleManager.Instance.StageIndex;
        
        stat.Initialize(data, stage);

        move   = MovementFactory.Create(data.EnemyMovementPattern, this);
        attack = AttackFactory.Create(data.Atk_Pattern, this);

        move?.Init(this);
        attack?.Init(this);
        attack?.Start();
    }

    void Update()
    {
        ApplyDoT(Time.deltaTime);
        stat?.Tick(Time.deltaTime);
        move?.Tick(Time.deltaTime);
        attack?.Tick(Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        move?.OnTriggerEnter2D(col);
        attack?.OnTriggerEnter2D(col);

        if ((attack is SuicideAttack) == false) // SuicideAttack는 딜 무시
        {
            int bit = 1 << col.gameObject.layer;
            if ((playerLayerMask.value & bit) != 0 && col.TryGetComponent<IBattleCharacter>(out var target))
            {
                BattleEventManager.CallEvent
                (
                    new TakeDamageEventArgs(this, target, Stat.FinalStat.Atk)
                );
            }
        }

        if (col.gameObject.layer == 9)  // ClearZone
        {
            var alivePoint = EnemyDataManager.Instance.Records[enemyID].Chasing_Increase;
            BattleEventManager.CallEvent(new AliveMonsterEventArgs(this, alivePoint));
            Destroy(gameObject);
        }
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        int raw   = Mathf.RoundToInt(eventArgs.Damage * State.ShieldRate); // ShieldAttack이 계산해서 넣어줌
        int final = stat.ApplyIncomingDamage(raw);
        var attacker = eventArgs.Attacker as MonoBehaviour;
        //Debug.Log($"[Monster] from {attacker.name} {EnemyID} took {eventArgs.Damage} * {State.ShieldRate} => {final} damage (raw: {raw}).");
        
        BattleEventManager.CallEvent(new ReceiveDamageEventArgs(this, final));

        if (stat.ReduceHP(final))
        {
            OnDeath();
        }
    }
    
    public void OnDeath()
    {
        if (deathEffectPrefab != null)
        {
            var effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * scale.ScaleFactor;
        }
        
        BattleEventManager.CallEvent(new DeathEventArgs(this));
        Destroy(gameObject);
    }

    void OnChargeCollision(ChargeCollisionArgs args)
    {
        if (ReferenceEquals(args.Target, this) == false)
        {
            return;
        }
        StartCoroutine(ApplyKnockback(args));
    }
    
    private void ApplyDoT(float time)
    {
        int totalDamage = 0;
        
        foreach (var dot in stat.GetModifiersOfType<DamageOverTimeModifier>())
        {
            totalDamage += dot.DamageTick(time);
        }

        if (totalDamage > 0)
        {
            var evt = new TakeDamageEventArgs(this, this, totalDamage);
            BattleEventManager.CallEvent(evt);
        }
    }

    IEnumerator ApplyKnockback(ChargeCollisionArgs args)
    {
        if ((args.Attacker is MonoBehaviour attackerMono) == false) yield break;

        Vector2 dir   = ((Vector2)transform.position - (Vector2)attackerMono.transform.position).normalized;
        float   speed = args.KnockBackDistance / knockbackDuration;
        float   time  = 0f;

        while (time < knockbackDuration)
        {
            rigid.velocity = dir * speed;
            time += Time.deltaTime;
            yield return null;
        }
        rigid.velocity = Vector2.zero;
    }

    // === 디버그 Gizmos ===
    void OnDrawGizmosSelected()
    {
        // Chase 감지 범위
        if (move is ChaseMove chaseMove && chaseCfg != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, chaseCfg.detectRange);
        }
        
        if (attack is SuicideAttack suicideAttack && suicideCfg != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, suicideCfg.attackRange);
        }

        // Shield 방어각
        if (attack is ShieldAttack shieldAttack && shieldCfg != null)
        {
            Gizmos.color = Color.blue;
            Vector2 origin = (Vector2)transform.position + shieldCfg.pivotOffset;
            Gizmos.DrawWireSphere(origin, shieldCfg.radius);

            float half = shieldCfg.angleDeg * 0.5f;
            Vector2 forward = Vector3.down * shieldCfg.radius;
            Vector2 left    = Quaternion.Euler(0, 0,  half) * forward;
            Vector2 right   = Quaternion.Euler(0, 0, -half) * forward;

            Gizmos.DrawLine(origin, origin + forward);
            Gizmos.DrawLine(origin, origin + left);
            Gizmos.DrawLine(origin, origin + right);
        }
    }
}

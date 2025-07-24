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
    [SerializeField] private EnemyID enemyID;           
    public EnemyID EnemyID => enemyID;                  
    public void SetEnemyID(EnemyID id) => enemyID = id; 

    public class MonsterRuntimeState
    {
        public bool  Detected;
        public bool  StoppedFlying;
        public float ShieldRate = 1f;
    }
    public MonsterRuntimeState State { get; } = new MonsterRuntimeState();
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
        BattleEventManager.Instance.Callbacks.OnChargeCollision += OnChargeCollision;
        BattleManager.Instance.RegisterMonster(this);
    }

    void OnDisable()
    {
        BattleEventManager.Instance.Callbacks.OnChargeCollision -= OnChargeCollision;
        BattleManager.Instance.UnregisterMonster(this);
    }

    void Start()
    {
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
        stat?.Tick(Time.deltaTime); // 디버프 만료 Tick 방식 도입 시 활성화
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
                BattleEventManager.Instance.CallEvent
                (
                    new TakeDamageEventArgs(this, target, Stat.FinalStat.Atk)
                );
            }
        }

        if (col.gameObject.layer == 9)  // ClearZone
        {
            BattleEventManager.Instance.CallEvent(new AliveMonsterEventArgs(this));
            Destroy(gameObject);
        }
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        int raw   = Mathf.RoundToInt(eventArgs.Damage * State.ShieldRate); // ShieldAttack이 계산해서 넣어줌
        int final = stat.ApplyIncomingDamage(raw);

        BattleEventManager.Instance.CallEvent(new ReceiveDamageEventArgs(this, final));

        if (stat.ReduceHP(final))
        {
            OnDeath();
        }
    }

    [Header("사망 이펙트")] [SerializeField] private GameObject deathEffectPrefab;
    public void OnDeath()
    {
        if (deathEffectPrefab != null)
        {
            var effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * scale.ScaleFactor;
        }
        
        BattleEventManager.Instance.CallEvent(new DeathEventArgs(this));
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

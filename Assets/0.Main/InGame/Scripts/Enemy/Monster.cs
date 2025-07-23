using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour, IBattleCharacter
{
    private MonsterStat stat;
    public MonsterStat Stat => stat;

    private IMoveBehaviour move;
    private IAttackBehaviour attack;
    
    [Header("데이터 연동")] [Tooltip("몬스터 유형 체크")] 
    [SerializeField] private EnemyID enemyID;
    
    // todo: 종류별 Prefab생성해서 관리하도록 수정필요. 수정 후 제거
    public EnemyID EnemyID { get { return enemyID;} set { enemyID = value; } } 
    
    [Header("플레이어 테스트")] [Tooltip("추적/충돌할 플레이어 오브젝트")]
    public GameObject Player;
    [Tooltip("플레이어 레이어")]
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private LayerMask projectileLayerMask;
    
    [Header("투사체")] [Tooltip("투사체 프리팹, 속도")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 5f;
    [SerializeField] private float fireInterval = 3f;
    private float mProjectileDamage;
    private bool mIsFirng = false;
    
    [Header("사망 시 애니메이션")]  // 직접 애니메이션 오브젝트 생성
    [SerializeField] private GameObject deathAnimationPrefab;
    
    [Header("지그재그")] [Tooltip("zigzagAmplitude: 진동폭, zigzagFrequency: 주기")]
    [SerializeField] private float zigzagAmplitude = 1f;
    [SerializeField] private float zigzagFrequency = 2f;
    [SerializeField] private float OffsetX = 0.5f; // 지그재그 이동 시 벽에 닿았을 때 X 좌표 오프셋

    private float mBaseLineX;        // 현재 진동 중심
    private float mTargetBaseLineX;  // 목표 진동 중심
    private float mZigzagTime;

    private float mNewBaseLineX; // 새 기준선
    private float mCycleLength;  // 1 사이클 = 2π/ω
    private int   mNextCycle;      
    private bool  mHasHitted;         
    private float mShiftSpeed;   // 수평 이동 속도

    [Header("자폭")] [Tooltip("자폭 딜레이, 탐지 범위, 데미지 처리 범위, 폭발 이펙트 설정")]
    [SerializeField] private float suicideDelay = 2f;
    [SerializeField] private float detectRange = 0.5f;
    [SerializeField] private float attackRange = 0.75f;
    [SerializeField] private GameObject explosionPrefab;
    private bool mIsDetected;
    private bool mIsSuicide;

    [Header("분사형")] [Tooltip("분사형 몬스터 발사 간격, 발사 횟수 설정")] 
    [SerializeField] private float spreadFireInterval = 0.2f;
    [SerializeField] private int spreadFireCount = 3;
    
    [Header("체공형")] [Tooltip("하강 후 멈출 거리 설정")]
    [SerializeField] private float distanceToStop = 3f;
    private float mFlyStartY;
    private bool mHasStoppedFlying;
    
    [Header("쉴드")] [Tooltip("쉴드 거리 & 각도 & 피벗")]
    [SerializeField] private float shieldRadius = 1f;
    [SerializeField] private float shieldAngleDeg = 90f;
    [SerializeField] private Vector2 shieldPivotOffset;
    private float mShieldRate = 1;
    
    [Header("넉백 설정")] [Tooltip("지속 시간, 넉백 세기는 Squad에")]
    [SerializeField] private float knockbackDuration = 0.2f;  // 넉백 지속 시간
    
    private Rigidbody2D rigid;
    
    private float mMoveSpeed;
    private int   mAttackPower;
    private int   mCurrentHP;
    private int   mMaxHP;
    
    private EnemyMovementPattern mMovementPattern;
    private EnemyAttackPattern   mAttackPattern;
    
    public int MaxHP => mMaxHP;
    public void SetPlayer(GameObject player) => Player = player;
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        stat = GetComponent<MonsterStat>();
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
    
    private void OnChargeCollision(ChargeCollisionArgs args)
    {
        if (ReferenceEquals(args.Target, this) == false)
        {
            return;
        }

        StartCoroutine(ApplyKnockback(args));
    }

    private IEnumerator ApplyKnockback(ChargeCollisionArgs args)
    {
        var attackerMono = args.Attacker as MonoBehaviour;
        
        if (attackerMono == null) yield break;

        Vector2 dir = ((Vector2)transform.position - (Vector2)attackerMono.transform.position).normalized;
        float speed = args.KnockBackDistance / knockbackDuration;
        float timer = 0f;

        while (timer < knockbackDuration)
        {
            rigid.velocity = dir * speed;
            timer += Time.deltaTime;
            yield return null;
        }

        rigid.velocity = Vector2.zero;
    }
    
    void Start()
    {
        var data = EnemyDataManager.Instance.Records[enemyID];
        
        mMoveSpeed       = data.Move_Speed;
        mAttackPower     = data.Atk_Power;
        mMaxHP           = data.HP;
        mCurrentHP       = mMaxHP;
        
        mMovementPattern = data.EnemyMovementPattern;
        mAttackPattern   = data.Atk_Pattern;
        
        mBaseLineX        = transform.position.x;
        mTargetBaseLineX  = mBaseLineX;
        mCycleLength       = 2f * Mathf.PI / zigzagFrequency;
        mHasHitted     = false;
        mShiftSpeed  = 0f;
        
        mFlyStartY = transform.position.y; // 체공형 초기 Y 좌표
    }

    void Update()
    {
        OnMove(mMovementPattern);
        OnAttack(mAttackPattern);
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        int damage = Mathf.RoundToInt(eventArgs.Damage * mShieldRate);
        
        BattleEventManager.Instance.CallEvent(new ReceiveDamageEventArgs(this, damage));
        
        mCurrentHP -= damage;
        
        if (mCurrentHP <= 0) // HP가 줄어서 사망했을 경우
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        // 죽음 이펙트 생성 후 파괴시키기 ? Instantiate(deathAnimationPrefab);
        BattleEventManager.Instance.CallEvent(new DeathEventArgs(this)); 
        Destroy(gameObject);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (mAttackPattern == EnemyAttackPattern.Suicide) // 자폭은 단순 충돌로 데미지를 입지 않도록
        {
            return;
        }
 
        // Border Object 충돌 시 (지그재그 처리)
        if (mMovementPattern == EnemyMovementPattern.Zigzag && collision.gameObject.layer == 8)
        {
            float middle = (MapManager.Instance.LeftBoundX + MapManager.Instance.RightBoundX) * 0.5f;
            float shift = (transform.position.x < middle) ? OffsetX : -OffsetX;
        
            mHasHitted    = true;
            mNewBaseLineX = mBaseLineX + shift;
            mNextCycle    = Mathf.FloorToInt(mZigzagTime / mCycleLength);
        }
        
        int bit = 1 << collision.gameObject.layer;
        if ((playerLayerMask.value & bit) != 0)
        {
            var player = collision.GetComponent<IBattleCharacter>();
            if (player != null)
            {
                BattleEventManager.Instance.CallEvent(
                    new TakeDamageEventArgs
                        (
                            attacker: this, 
                            target: player, 
                            damage: mAttackPower
                        )
                );
            }
        }
        
        // ClearZone
        if (collision.gameObject.layer == 9)    
        {
            BattleEventManager.Instance.CallEvent(new AliveMonsterEventArgs(this));
            Destroy(gameObject);
        }
    }

    // 몬스터 이동 로직
    public void OnMove(EnemyMovementPattern movementType) 
    {
        if (mIsDetected)
        {
            return;
        }
        
        switch (movementType)
        {
            case EnemyMovementPattern.Straight:
                
                transform.position += Vector3.down * (mMoveSpeed * Time.deltaTime);
                
                break;
            
            case EnemyMovementPattern.Zigzag:
                
                mZigzagTime += Time.deltaTime;

                // 벽 부딪히면 OffsetX만큼 기준선 이동
                if (mHasHitted)
                {
                    int currentCycle = Mathf.FloorToInt(mZigzagTime / mCycleLength);
                    
                    if (currentCycle >= mNextCycle)
                    {
                        mHasHitted      = false;
                        mTargetBaseLineX    = mNewBaseLineX;
                        float shiftDist    = mTargetBaseLineX - mBaseLineX;
                        mShiftSpeed   = Mathf.Abs(shiftDist) / mCycleLength;    // 기준선 이동 속도
                    }
                }

                // 수평 기준선 이동
                mBaseLineX = Mathf.MoveTowards
                (
                    mBaseLineX, mTargetBaseLineX, mShiftSpeed * Time.deltaTime
                );

                float x = mBaseLineX + Mathf.Sin(mZigzagTime * zigzagFrequency) * zigzagAmplitude;
                float y = transform.position.y - mMoveSpeed * Time.deltaTime;
                transform.position = new Vector2(x, y);
                
                break;
            
            case EnemyMovementPattern.Chase:
                
                var hits = Physics2D.OverlapCircleAll(transform.position, detectRange, playerLayerMask);
                
                if (hits.Length > 0)
                {
                    mIsDetected = true;
                    break;
                }
                
                if (Player != null)
                {
                    transform.position = Vector2.MoveTowards
                    (
                        transform.position,
                        Player.transform.position,
                        mMoveSpeed * Time.deltaTime
                    );
                }
                else
                {
                    transform.position += Vector3.down * (mMoveSpeed * Time.deltaTime);
                }
                
                break;

            case EnemyMovementPattern.Flying:
                
                float traveled = mFlyStartY - transform.position.y;
                
                if (traveled < distanceToStop)
                {
                    transform.position += Vector3.down * (mMoveSpeed * Time.deltaTime);
                }
                else
                {
                    mHasStoppedFlying = true;
                }
                
                break;
        }
    }
    
    // 몬스터 공격 로직
    public void OnAttack(EnemyAttackPattern attackPattern)
    {
        switch (attackPattern)
        {
            case EnemyAttackPattern.Normal:
                
                break;
            
            case EnemyAttackPattern.Suicide:
                
                if (mIsDetected && mIsSuicide == false)
                {
                    mIsSuicide = true;        
                    StartCoroutine(OnSuicide());
                }
                
                break;
            
            case EnemyAttackPattern.Shield:
                
                ApplyShieldRate();
                
                break;
            
            case EnemyAttackPattern.Sniper:
                
                if (mIsFirng == false)
                {
                    mIsFirng = true;
                   StartCoroutine(SniperAttackLoop());
                }
                
                break;
            
            case EnemyAttackPattern.Spread:
                
                if (mIsFirng == false)
                {
                    mIsFirng = true;
                    StartCoroutine(SpreadAttackLoop());
                }
                
                break;
            
            case EnemyAttackPattern.Radial:
                
                if (mIsFirng == false)
                {
                    mIsFirng = true;
                    StartCoroutine(RadialAttackLoop());
                }
                
                break;
            
            case EnemyAttackPattern.Flying:
                
                if (mHasStoppedFlying && mIsFirng == false)
                {
                    mIsFirng = true;
                    StartCoroutine(FlyingAttackLoop());
                }
                
                break;
        }
    }
    
    IEnumerator OnSuicide()
    {
        // 추가 : Fade 처리하면 좋을 듯, 빨간색으로 깜빡깜빡,
        // BattleEventManager.Instance.CallEvent(new SuicideEvent(this, suicideDelay)); 이런 식으로 ?
        
        yield return new WaitForSeconds(suicideDelay);

        var exposionObject = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        var explosion = exposionObject.GetComponent<EnemyExplosion>();

        explosion.Initialize
        (
            attacker: this,
            damage:   mAttackPower,
            radius:   attackRange,
            mask:     playerLayerMask
        );

        Destroy(gameObject);
    }

    private void ApplyShieldRate()
    {
        mShieldRate = 1f;
        
        Vector2 origin = (Vector2)transform.position + shieldPivotOffset;
        float halfCos = Mathf.Cos((shieldAngleDeg * 0.5f) * Mathf.Deg2Rad);
        
        LayerMask comboMask = playerLayerMask | projectileLayerMask;
        var hits = Physics2D.OverlapCircleAll(origin, shieldRadius, comboMask);

        foreach (var col in hits)
        {
            Vector2 hitPos = col.ClosestPoint(origin);
            Vector2 dir    = (hitPos - origin).normalized;
            Vector2 forward = -transform.up; 

            float dot  = Vector2.Dot(forward, dir);
            float distance = Vector2.Distance(origin, hitPos);

            if (dot >= halfCos && distance <= shieldRadius)
            {
                mShieldRate = 0.5f;
                break;
            }
        }
    }
    
    private Vector2 SetFireAngle(Vector2 vector, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        
        float cosX = Mathf.Cos(rad);
        float sinX = Mathf.Sin(rad);
        
        return new Vector2(vector.x * cosX - vector.y * sinX, vector.x * sinX + vector.y * cosX);
    }

    private IEnumerator FireProjectiles(int count, float interval, Func<int, Vector2> aim)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            var proj= obj.GetComponent<Projectile>();
            
            proj.Initialize
            (
                dir:      aim(i),
                speed:    projectileSpeed,
                damage:   (int)mAttackPower,
                mask:     playerLayerMask,
                attacker: this
            );
            
            if (interval > 0f)
            {
                yield return new WaitForSeconds(interval);
            }
            else
            {
                yield return null;
            }
        }
    }
    
    private IEnumerator SniperAttackLoop()
    {
        while (true)
        {
            yield return FireProjectiles
            (
                count:    3,
                interval: 0.15f,
                aim: i => (Player.transform.position - transform.position).normalized
            );
            
            yield return new WaitForSeconds(fireInterval);
        }
    }
    
    private IEnumerator SpreadAttackLoop()
    {
        for (int i = 0; i < spreadFireCount; i++)
        {
            bool isLeft = Player.transform.position.x < transform.position.x;
            float[] angles = isLeft ? new[] {  0f, -15f, -30f, -45f } : new[] {  0f, 15f, 30f,  45f };
            
            yield return FireProjectiles
            (
                count:    4,
                interval: 0f,
                aim: i => SetFireAngle(Vector2.down, angles[i])
            );
            
            yield return new WaitForSeconds(spreadFireInterval);
        }
    }
    
    private IEnumerator RadialAttackLoop()
    {
        while (true)
        {
            yield return FireProjectiles
            (
                count:    12,
                interval: 0f,
                aim: i =>
                {
                    float angle = i * 30f;
                    return SetFireAngle(Vector2.down, angle);
                }
            );
            yield return new WaitForSeconds(fireInterval);
        }
    }

    private IEnumerator FlyingAttackLoop()
    {
        while (true)
        {
            float range = Random.Range(0f, 1f);
            Vector2 target = Vector2.Lerp
            (
                MapManager.Instance.MidLeft.position,
                MapManager.Instance.MidRight.position,
                range
            );

            Vector2 dir = (target - (Vector2)transform.position).normalized;

            yield return FireProjectiles
            (
                count:    1,
                interval: 0f,
                aim: i => dir
            );

            // 대기, 얘만 1초 (나중에 기획 쪽 수정 필요 : fireInterval)
            yield return new WaitForSeconds(1f);
        }
    }
    
    // 감지 범위
    void OnDrawGizmosSelected()
    {
        if (mMovementPattern == EnemyMovementPattern.Chase)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
        
        if (mAttackPattern == EnemyAttackPattern.Shield)
        {
            Gizmos.color = Color.blue;
            Vector2 origin = (Vector2)transform.position + shieldPivotOffset;
            
            Gizmos.DrawWireSphere(origin, shieldRadius);
            float half = shieldAngleDeg * 0.5f;
            Vector2 forward = Vector3.down * shieldRadius;
            
            Vector2 left    = Quaternion.Euler(0, 0,  half) * forward;
            Vector2 right   = Quaternion.Euler(0, 0, -half) * forward;
            
            Gizmos.DrawLine(origin, origin + forward);
            Gizmos.DrawLine(origin, origin + left);
            Gizmos.DrawLine(origin, origin + right);
        }
    }
}
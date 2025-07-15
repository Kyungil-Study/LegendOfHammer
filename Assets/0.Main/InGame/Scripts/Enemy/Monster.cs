using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour, IBattleCharacter
{
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
    
    [Header("지그재그")] [Tooltip("zigzagAmplitude: 진동폭, zigzagFrequency: 주기")]
    [SerializeField] private float zigzagAmplitude = 1f;
    [SerializeField] private float zigzagFrequency = 2f;
    private float mInitialX;
    private float mZigzagTime;

    [Header("자폭")] [Tooltip("자폭 딜레이, 탐지 범위, 데미지 처리 범위, 폭발 이펙트 설정")]
    [SerializeField] private float suicideDelay = 2f;
    [SerializeField] private float detectRange = 0.5f;
    [SerializeField] private float attackRange = 0.75f;
    [SerializeField] private GameObject explosionPrefab;
    private bool mIsDetected;
    private bool mIsSuicide;
    
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
    private float knockbackForce;
    
    private Rigidbody2D rigid;
    
    private float mMoveSpeed;
    private int   mAttackPower;
    private int   mCurrentHP;
    private int   mMaxHP;
    public int MaxHP => mMaxHP;
    
    private EnemyMovementPattern mMovementPattern;
    private EnemyAttackPattern   mAttackPattern;
    
    public void SetPlayer(GameObject player) => Player = player;
    
    void Awake()
    {
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
        if (attackerMono == null)
        {
            yield break;
        }

        Vector2 dir = ((Vector2)transform.position - (Vector2)attackerMono.transform.position).normalized;
        float timer = 0f;

        while (timer < knockbackDuration)
        {
            rigid.velocity = dir * args.KnockBackDistance;
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
        
        mInitialX = transform.position.x; // 지그재그 초기 X 좌표
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
            mInitialX = transform.position.x;

            float width = transform.position.x;
            float mid = (MapManager.Instance.LeftBoundX + MapManager.Instance.RightBoundX) * 0.5f;

            if (width < mid)
            {
                // 왼쪽 벽: 위상 0 → 오른쪽으로 튕기도록
                mZigzagTime = 0f;
            }
            else
            {
                // 오른쪽 벽: 위상을 π/ω 로 → 왼쪽으로 튕기도록
                mZigzagTime = Mathf.PI / zigzagFrequency;
            }
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
                
                float x = mInitialX + Mathf.Sin(mZigzagTime * zigzagFrequency) * zigzagAmplitude;
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
        // 추가 : Fade 처리하면 좋을 듯, 빨간색으로 깜빡깜빡
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
        while (true)
        {
            bool isLeft = Player.transform.position.x < transform.position.x;
            float[] angles = isLeft ? new[] {  0f, -27.5f, -45f } : new[] {  0f,  27.5f,  45f };
            
            yield return FireProjectiles
            (
                count:    3,
                interval: 0.2f,
                aim: i => SetFireAngle(Vector2.down, angles[i])
            );
            yield return new WaitForSeconds(fireInterval);
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
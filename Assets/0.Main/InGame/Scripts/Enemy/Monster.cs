using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour, IBattleCharacter
{
    [Header("데이터 연동")]
    [Tooltip("몬스터 유형 체크")] 
    [SerializeField] private EnemyID enemyID;
    
    // todo: 종류별 Prefab생성해서 관리하도록 수정필요. 수정 후 제거
    public EnemyID EnemyID { get { return enemyID;} set { enemyID = value; } } 
    
    [Header("플레이어 테스트")] [Tooltip("추적/충돌할 플레이어 오브젝트")]
    public GameObject testPlayer;
    [Tooltip("플레이어 레이어 마스크")]
    [SerializeField] private LayerMask testPlayerLayerMask;
    
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
    
    [Header("쉴드")] [Tooltip("쉴드 거리")]
    [SerializeField] private float shieldDistance = 1f;
    
    [Header("넉백 설정")] [Tooltip("넉백 세기, 지속 시간")]
    [SerializeField] private float knockbackForce    = 2.5f;    // 넉백 세기
    [SerializeField] private float knockbackDuration = 0.2f;  // 넉백 지속 시간

    private Rigidbody2D rigid;
    
    private float mMoveSpeed;
    private int   mAttackPower;
    private int   mMaxHP;
    private int   mCurrentHP;
    
    private EnemyMovementPattern mMovementPattern;
    private EnemyAttackPattern   mAttackPattern;
    
    public void SetPlayer(GameObject player) => testPlayer = player;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        BattleEventManager.Instance.Callbacks.OnChargeCollision += OnChargeCollision;
    }

    void OnDisable()
    {
        BattleEventManager.Instance.Callbacks.OnChargeCollision -= OnChargeCollision;
    }
    
    private void OnChargeCollision(ChargeCollisionArgs args)
    {
        if (ReferenceEquals(args.Target, this) == false) return;

        StartCoroutine(ApplyKnockback(args));
    }

    // 넉백 관련 코드, 나중에 테스트 해볼 필요 있음
    private IEnumerator ApplyKnockback(ChargeCollisionArgs args)
    {
        var attackerMb = args.Attacker as MonoBehaviour;
        if (attackerMb == null)
        {
            Debug.Log("넉백 에러, 코드 수정 필요");
            yield break;
        }

        Vector2 dir = ((Vector2)transform.position - (Vector2)attackerMb.transform.position).normalized;
        float timer = 0f;

        while (timer < knockbackDuration)
        {
            rigid.velocity = dir * knockbackForce;
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
        mMaxHP           = data.HP ?? 0;
        mCurrentHP       = mMaxHP;
        
        mMovementPattern = data.EnemyMovementPattern;
        mAttackPattern   = data.Atk_Pattern;
        
        mInitialX = transform.position.x; // 지그재그 초기 X 좌표 저장
        mFlyStartY = transform.position.y; // 체공형 초기 Y 좌표 저장
    }

    void Update()
    {
        OnMove(mMovementPattern);
        OnAttack(mAttackPattern);
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        int damage = eventArgs.Damage;

        // Shield 패턴일 때 반감처리
        if (mAttackPattern == EnemyAttackPattern.Shield)
        {
            if (eventArgs.Attacker is MonoBehaviour mono)
            {
                Vector2 attackerPos = mono.transform.position;
                Vector2 myPos = transform.position;
                Vector2 hitDir = (attackerPos - myPos).normalized;
                Vector2 forward = Vector2.down;

                float dot = Vector2.Dot(forward, hitDir);

                if (dot >= Mathf.Cos(45f * Mathf.Deg2Rad)
                    && Vector2.Distance(attackerPos, myPos) <= shieldDistance)
                {
                    damage = Mathf.CeilToInt(damage * 0.5f);
                    // 나중에 확인할 수 있도록, 플레이어 -> 적 공격 시 로그로 확인
                    Debug.Log($"Shield 반감 적용: 원래 데미지 : {eventArgs.Damage} // 적용 데미지 : {damage}");
                }
            }
        }

        mCurrentHP -= damage;
        Debug.Log($"TakeDamage 호출 : 받은 데미지 ? => {damage} // 남은 HP ? => {mCurrentHP}");
        
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

            // 좌/우 벽 판단
            float x = transform.position.x;
            float mid = (MapManager.Instance.LeftBoundX + MapManager.Instance.RightBoundX) * 0.5f;

            if (x < mid)
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
        
        // 플레이어 레이어 체크, 충돌 시 플레이어 쪽 데미지 Call
        int bit = 1 << collision.gameObject.layer;
        if ((testPlayerLayerMask.value & bit) != 0)
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
                Debug.Log($"플레이어 받은 데미지 : {mAttackPower}");
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
                
                var hits = Physics2D.OverlapCircleAll(transform.position, detectRange, testPlayerLayerMask);
                
                if (hits.Length > 0)
                {
                    mIsDetected = true;
                    break;
                }
                
                if (testPlayer != null)
                {
                    transform.position = Vector2.MoveTowards
                    (
                        transform.position,
                        testPlayer.transform.position,
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
            
            case EnemyAttackPattern.Shield: // TakeDamage()에서 반감 구현
                
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

        var expObj = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        var explosion = expObj.GetComponent<EnemyExplosion>();

        explosion.Initialize
        (
            attacker: this,
            damage:   mAttackPower,
            radius:   attackRange,
            mask:     testPlayerLayerMask
        );

        Destroy(gameObject);
    }

    private Vector2 SetFireAngle(Vector2 vector, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cosX = Mathf.Cos(rad), sinX = Mathf.Sin(rad);
        
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
                mask:     testPlayerLayerMask,
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
                aim: i => (testPlayer.transform.position - transform.position).normalized
            );
            
            yield return new WaitForSeconds(fireInterval);
        }
    }
    
    private IEnumerator SpreadAttackLoop()
    {
        while (true)
        {
            bool isLeft = testPlayer.transform.position.x < transform.position.x;
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
        
        // Shield 원뿔 범위 시각화
        if (mAttackPattern == EnemyAttackPattern.Shield)
        {
            Gizmos.color = Color.blue;
            Vector3 origin = transform.position;
            
            float halfAngle = 45f; 
            int segments = 20;

            Vector3 forward = Vector3.down;
            Vector3 leftDir = Quaternion.Euler(0, 0, halfAngle) * forward;
            Vector3 rightDir = Quaternion.Euler(0, 0, -halfAngle) * forward;

            Gizmos.DrawLine(origin, origin + forward * shieldDistance);
            Gizmos.DrawLine(origin, origin + leftDir * shieldDistance);
            Gizmos.DrawLine(origin, origin + rightDir * shieldDistance);

            Vector3 prevPoint = origin + leftDir * shieldDistance;
            for (int i = 1; i <= segments; i++)
            {
                float angle = halfAngle - (i * (halfAngle * 2) / segments);
                Vector3 dir = Quaternion.Euler(0, 0, angle) * forward;
                Vector3 nextPoint = origin + dir * shieldDistance;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
    }
}
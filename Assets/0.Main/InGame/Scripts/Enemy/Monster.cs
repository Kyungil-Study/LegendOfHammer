using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float mProjectileDamage;
    
    [Header("지그재그")] [Tooltip("zigzagAmplitude: 진동폭, zigzagFrequency: 주기")]
    [SerializeField] private float zigzagAmplitude = 1f;
    [SerializeField] private float zigzagFrequency = 2f;

    [Header("자폭")] [Tooltip("자폭 딜레이, 탐지 범위, 데미지 처리 범위, 폭발 이펙트 설정")]
    [SerializeField] private float suicideDelay = 2f;
    [SerializeField] private float detectRange = 0.5f;
    [SerializeField] private float attackRange = 0.75f;
    [SerializeField] private GameObject explosionPrefab;
    
    [Header("체공형")] [Tooltip("하강 후 멈출 거리 설정")]
    [SerializeField] private float distanceToStop = 3f;
    private float mFlyStartY;
    private bool mHasStoppedFlying;
    
    [Header("쉴드")] [Tooltip("쉴드 거리")]
    [SerializeField] private float shieldDistance = 1f;
    
    private float mMoveSpeed;
    private int   mAttackPower;
    private int   mMaxHP;
    private int   mCurrentHP;
    
    private EnemyMovementPattern mMovementPattern;
    private EnemyAttackPattern   mAttackPattern;
    
    // 지그재그용
    private float mInitialX;
    private float mZigzagTime;
    
    // 추적, 자폭용
    private bool mIsDetected;
    private bool mIsSuicide;

    public void SetPlayer(GameObject player) => testPlayer = player;
    
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
                Vector2 myPos       = transform.position;
                Vector2 hitDir = (attackerPos - myPos).normalized;
                Vector2 forward = Vector2.down;

                float dot = Vector2.Dot(forward, hitDir);
                
                if (dot >= Mathf.Cos(45f * Mathf.Deg2Rad)
                    && Vector2.Distance(attackerPos, myPos) <= shieldDistance)
                {
                    damage = Mathf.CeilToInt(damage * 0.5f);
                    // 나중에 확인할 수 있도록, 플레이어 -> 적 공격 시 로그로
                    Debug.Log($"Shield 반감 적용: 원래 데미지 : {eventArgs.Damage} // 적용 데미지 : {damage}");
                }
            }
        }

        mCurrentHP -= damage;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Suicide 패턴은 별도 처리
        if (mAttackPattern == EnemyAttackPattern.Suicide)
        {
            return;
        }
        
        // 플레이어 레이어 체크
        int bit = 1 << collision.gameObject.layer;
        if ((testPlayerLayerMask.value & bit) == 0)
        {
            return;
        }
        
        Debug.Log("플레이어 충돌 시 데미지 처리");
        
        var player = collision.GetComponent<IBattleCharacter>();
        
        if (player != null)
        {
            BattleEventManager.Instance.CallEvent
            (
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

    // 몬스터 이동 로직
    public void OnMove(EnemyMovementPattern movementType) 
    {
        if (mIsDetected)
        {
            return;
        }
        
        switch (movementType)
        {
            case EnemyMovementPattern.Straight: // 등속 직선 이동 (하강)
                transform.position += Vector3.down * (mMoveSpeed * Time.deltaTime);
                break;
            case EnemyMovementPattern.Zigzag: // 지그재그 이동 : 진동폭 만큼 좌우로 움직이며 등속 직선 이동 (하강)
                mZigzagTime += Time.deltaTime;
                float x = mInitialX + Mathf.Sin(mZigzagTime * zigzagFrequency) * zigzagAmplitude;
                float y = transform.position.y - mMoveSpeed * Time.deltaTime;
                transform.position = new Vector2(x, y);
                break;
            case EnemyMovementPattern.Chase:
                // 추적 이동 : 플레이어 추적 -> 일정 트리거 범위에서 멈추고 폭발 (이건 OnAttack서 처리)
                // 플레이어 추적, 없으면 하강
                var hits = Physics2D.OverlapCircleAll(transform.position, detectRange, testPlayerLayerMask);
                
                if (hits.Length > 0)
                {
                    mIsDetected = true;
                    // TODO: 폭발 준비 (OnAttack에서 처리)
                    break;
                }
                
                if (testPlayer != null)
                {
                    transform.position = Vector2.MoveTowards(
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
                // 체공형 : OnAttack 시 멈춰서 발사
                // 전장 하단으로 이동 → distanceToStop 이하 도달 시 멈춤, 멈추면 발사 시작
                float traveled = mFlyStartY - transform.position.y;
                if (traveled < distanceToStop)
                {
                    transform.position += Vector3.down * (mMoveSpeed * Time.deltaTime);
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
                // 일반 공격, 그냥 닿으면 플레이어 TakeDamage 처리
                break;
            case EnemyAttackPattern.Suicide:
                // 플레이어 도달 시 일정 시간 후 폭발, 얘만 일반 공격으로 충돌 시 데미지 입히지 않음
                if (mIsDetected && mIsSuicide == false)
                {
                    mIsSuicide = true;        
                    StartCoroutine(OnSuicide());
                }
                break;
            case EnemyAttackPattern.Shield:
                // 공격 패턴이 아니라서, TakeDamage()에서 반감 로직 구현함
                break;
            case EnemyAttackPattern.Sniper:
                // 사격 시간 (3초) 마다 투사체 3연발(0.15초 간격)로 플레이어 방향으로 발사
                StartCoroutine(SniperAttackLoop());
                break;
            case EnemyAttackPattern.Spread:
                // 몬스터 전방 기준 45도 3갈래 3연발(0.2초 간격)로 발사
                // 탄은 탄막 속도로 등속 직선 이동
                // 몬스터와 플레이어 캐릭터의 상대적 위치에 따라 좌우 분사 방향 결정
                StartCoroutine(SpreadAttackLoop());
                break;
            case EnemyAttackPattern.Radial:
                // 몬스터 생성 후 사격 시간(3초) 마다 탄막 발사
                // 몬스터 기준 시계 12개 방향으로 발사 (원형)
                StartCoroutine(RadialAttackLoop());
                break;
            case EnemyAttackPattern.Flying:
                // 정지 후 사격 시간(1초) 마다 탄막 발사
                // 전장의 중선(파란선)을 지나는 무작위 방향으로 탄 1발 발사
                if (mHasStoppedFlying == false && HasFinishedFlying())
                {
                    mHasStoppedFlying = true;
                    StartCoroutine(FireProjectiles(
                        count:    1,
                        interval: 0f,
                        aim: i =>
                        {
                            // 0° 기준 오른쪽 → 랜덤 ±90°
                            float angle = Random.Range(-45f, 45f);
                            return SetAngle(Vector2.right, angle);
                        }
                    ));
                }
                break;
        }
    }
    
    IEnumerator OnSuicide()
    {
        // 추가 : 반짝반짝 거리게 Fade 처리
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
    
    // 발사 공통 메서드
    private IEnumerator FireProjectiles(int count, float interval, System.Func<int, Vector2> aim)
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
            yield return new WaitForSeconds(3f);
        }
    }
    
    private IEnumerator SpreadAttackLoop()
    {
        while (true)
        {
            yield return FireProjectiles
            (
                count:    3,
                interval: 0.2f,
                aim: i =>
                {
                    // 플레이어가 왼쪽이면 +, 오른쪽이면 −
                    bool isLeft = testPlayer.transform.position.x < transform.position.x;
                    float[] angles = isLeft ? new[] {0f, 27.5f, 45f} : new[] {0f, -27.5f, -45f};
                    return SetAngle(Vector2.down, angles[i]);
                }
            );
            yield return new WaitForSeconds(3f);
        }
    }
    
    // Radial 전용: 12방향 → 3초마다 발사
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
                    float angle = i * 30f; // 360/12 = 30
                    return SetAngle(Vector2.down, angle);
                }
            );
            yield return new WaitForSeconds(3f);
        }
    }

    private bool HasFinishedFlying()
    {
        // ex) y 위치가 목표 이하일 때
        float traveled = mFlyStartY - transform.position.y;
        return traveled >= distanceToStop;
    }

    private Vector2 SetAngle(Vector2 vector, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cx = Mathf.Cos(rad), sx = Mathf.Sin(rad);
        return new Vector2(vector.x * cx - vector.y * sx, vector.x * sx + vector.y * cx);
    }
    
    // Chase, Shield 감지 범위
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
            
            float halfAngle = 45f; // 90° 원뿔의 절반
            int segments = 20; // 호(arc) 분할 수

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
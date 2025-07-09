using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour, IBattleCharacter
{
    [Header("데이터 연동")]
    [Tooltip("몬스터 유형 체크")] 
    [SerializeField] private EnemyID enemyID;

    [Header("플레이어 테스트")] [Tooltip("추적/충돌할 플레이어 오브젝트")]
    public GameObject testPlayer;
    [Tooltip("플레이어 레이어 마스크")]
    [SerializeField] private LayerMask testPlayerLayerMask;
    
    [Header("투사체")]
    [SerializeField] private GameObject projectilePrefab;
    
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
    private float flyStartY;
    
    private float mMoveSpeed;
    private int   mAttackPower;
    private int   mMaxHP;
    private int   mCurrentHP;
    
    private EnemyMovementPattern mMovementPattern;
    private EnemyAttackPattern   mAttackPattern;
    
    // 지그재그용
    private float initialX;
    private float zigzagTime;
    
    // 자폭용
    private bool isSuiciding;

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
        
        // 지그재그 초기 X 좌표 저장
        initialX = transform.position.x;
        // 체공형 초기 Y 좌표 저장
        flyStartY = transform.position.y;
    }

    void Update()
    {
        OnMove(mMovementPattern);
        OnAttack(mAttackPattern);
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        mCurrentHP -= eventArgs.Damage;
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
        if (isSuiciding)
        {
            return;
        }
        
        switch (movementType)
        {
            case EnemyMovementPattern.Straight:
                // 등속 직선 이동 (하강)
                transform.position += Vector3.down * (mMoveSpeed * Time.deltaTime);
                break;
            case EnemyMovementPattern.Zigzag:
                // 지그재그 이동 : 진동폭 만큼 좌우로 움직이며 등속 직선 이동 (하강)
                zigzagTime += Time.deltaTime;
                float x = initialX + Mathf.Sin(zigzagTime * zigzagFrequency) * zigzagAmplitude;
                float y = transform.position.y - mMoveSpeed * Time.deltaTime;
                transform.position = new Vector2(x, y);
                break;
            case EnemyMovementPattern.Chase:
                // 추적 이동 : 플레이어 추적 -> 일정 트리거 범위에서 멈추고 폭발 (이건 OnAttack서 처리)
                // 플레이어 추적, 없으면 하강
                var hits = Physics2D.OverlapCircleAll(transform.position, detectRange, testPlayerLayerMask);
                
                if (hits.Length > 0)
                {
                    isSuiciding = true;
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
                float traveled = flyStartY - transform.position.y;
                if (traveled < distanceToStop)
                {
                    transform.position += Vector3.down * (mMoveSpeed * Time.deltaTime);
                }
                break;
            default:
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
                if (isSuiciding)
                {
                    StartCoroutine(OnSuicide());
                }
                break;
            case EnemyAttackPattern.Shield:
                // 전방 90도 범위 플레이어 공격 반감, 벡터 내적 활용할 계획 
                break;
            case EnemyAttackPattern.Sniper:
                // 사격 시간 (3초) 마다 투사체 발사
                break;
            case EnemyAttackPattern.Spread:
                // 몬스터 전방 기준 45도 3갈래 3연발(0.2초 간격)로 발사
                // 탄은 탄막 속도로 등속 직선 이동
                // 몬스터와 플레이어 캐릭터의 상대적 위치에 따라 좌우 분사 방향 결정
                break;
            case EnemyAttackPattern.Radial:
                // 몬스터 생성 후 사격 시간(3초) 마다 탄막 발사
                // 몬스터 기준 시계 12개 방향으로 발사 (원형)
                break;
            case EnemyAttackPattern.Flying:
                // 정지 후 사격 시간(1초) 마다 탄막 발사
                // 전장의 중선(파란선)을 지나는 무작위 방향으로 탄 1발 발사
                break;
            default:
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
    
    // Chase 형태에서 감지 범위
    void OnDrawGizmosSelected()
    {
        if (mMovementPattern == EnemyMovementPattern.Chase)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
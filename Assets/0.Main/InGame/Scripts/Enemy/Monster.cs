using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour, IBattleCharacter
{
    [Header("데이터 연동")]
    [Tooltip("몬스터 유형 체크")] 
    [SerializeField] private EnemyID enemyID;

    [Header("테스트 플레이어")]
    [SerializeField] private LayerMask testPlayer;
    
    [Header("투사체")]
    [SerializeField] private GameObject projectilePrefab;

    private float mMoveSpeed;
    private int   mMaxHP;
    private int   mCurrentHP;
    
    private EnemyMovementPattern mMovementPattern;
    private EnemyAttackPattern   mAttackPattern;
    
    void Start()
    {
        var data = EnemyDataManager.Instance.Records[enemyID];
        
        mMoveSpeed       = data.Move_Speed;
        mMaxHP           = data.HP ?? 0;
        mCurrentHP       = mMaxHP;
        
        mMovementPattern = data.EnemyMovementPattern;
        mAttackPattern   = data.Atk_Pattern;
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
        if (testPlayer == (testPlayer | (1 << collision.gameObject.layer)))
        {
            Debug.Log("플레이어 충돌");
            // TODO: 플레이어 데미지 이벤트 호출
        }
    }

    // 몬스터 이동 로직
    public void OnMove(EnemyMovementPattern movementType) 
    {
        switch (movementType)
        {
            case EnemyMovementPattern.Straight:
                // 등속 직선 이동 (하강)
                transform.position += Vector3.down * (mMoveSpeed * Time.deltaTime);
                break;
            case EnemyMovementPattern.Zigzag:
                // 지그재그 이동 : 진동폭 만큼 좌우로 움직이며 등속 직선 이동 (하강)
                break;
            case EnemyMovementPattern.Chase:
                // 추적 이동 : 플레이어 추적 -> 일정 트리거 범위에서 멈추고 폭발 (이건 OnAttack서 처리)
                break;
            case EnemyMovementPattern.Flying:
                // 체공형 : OnAttack 시 멈춰서 발사
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
}
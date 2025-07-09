using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour, IBattleCharacter
{
    [Header("테스트 플레이어")]
    [SerializeField] private LayerMask testPlayer;

    [Header("데이터 연동")] [Tooltip("몬스터 유형 체크")] 
    [SerializeField] private EnemyID enemyID;

    private float moveSpeed;

    void Start()
    {
        moveSpeed = EnemyDataManager.Instance.Records[enemyID].Move_Speed;
    }

    void Update()
    {
        OnMove(EnemyDataManager.Instance.Records[enemyID].EnemyMovementPattern);
        OnAttack(EnemyDataManager.Instance.Records[enemyID].Atk_Pattern);
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (testPlayer == (testPlayer | (1 << collision.gameObject.layer)))
        {
            Debug.Log("플레이어 충돌");
        }
    }

    public void OnMove(EnemyMovementPattern movementType)
    {
        // 몬스터 이동 로직 구현
        // 예: 이동 패턴에 따라 방향 전환 등
        
        switch (movementType)
        {
            case EnemyMovementPattern.Straight:
                transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
                break;
            case EnemyMovementPattern.Zigzag:
                // 지그재그 이동 로직
                break;
            case EnemyMovementPattern.Chase:
                // 추적 이동 로직
                break;
            case EnemyMovementPattern.Flying:
                // 추적 이동 로직
                break;
            default:
                break;
        }
    }
    
    public void OnAttack(EnemyAttackPattern attackPattern)
    {
        // 몬스터 공격 로직 구현
        // 예: 공격 패턴에 따라 공격 방식 결정
        switch (attackPattern)
        {
            case EnemyAttackPattern.Normal:
                // 일반 공격 
                break;
            case EnemyAttackPattern.Sniper:
                break;
            case EnemyAttackPattern.Flying:
                break;
            case EnemyAttackPattern.Radial:
                break;
            case EnemyAttackPattern.Shield:
                break;
            case EnemyAttackPattern.Spread:
                break;
            default:
                break;
        }
    }
}
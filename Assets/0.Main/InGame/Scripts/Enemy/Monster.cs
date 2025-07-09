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
        transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (testPlayer == (testPlayer | (1 << collision.gameObject.layer)))
        {
            Debug.Log("몬스터와 충돌");
        }
    }
}
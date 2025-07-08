using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour, IBattleCharacter
{
    [Header("데이터 연동")]
    [Tooltip("EnemyConfigLoader 에서 로드된 CSV 데이터의 EnemyID")]
    [SerializeField] private int enemyID;
    
    private float moveSpeed;

    void Start()
    {
        moveSpeed = 1f;
    }

    void Update()
    {
        transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
    }
}
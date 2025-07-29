using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeteorSpawner : MonoSingleton<MeteorSpawner>
{
    [Range(0,1), SerializeField] private float spawnTrigger = 0.3f; // 스폰 트리거 값
    [SerializeField] private Meteor meteorPrefab; // 메테오 프리팹
    [SerializeField] private Vector2 BoundSize;
    [SerializeField] private Vector2 BoundOffset;
    [SerializeField,Tooltip("플레이어 중심 반지름")] private float spawnRadius;
    [SerializeField, Tooltip("스폰어가 활성화된 상태에서만 메테오를 생성합니다.")] private bool isActive = true;
    [LabelText("메테오 스폰 간격"), SerializeField] private float spawnInterval = 1f; // 메테오 생성 간격

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)BoundOffset, BoundSize);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    public void ExecuteMapEvent(int damage, int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            var randCircle = Random.insideUnitCircle * spawnRadius;
            var spawnPosition = Squad.Instance.transform.position + new Vector3(randCircle.x , randCircle.y , 0); 
            
            Meteor meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
            meteor.transform.SetParent(transform);
        }
       
    }
}

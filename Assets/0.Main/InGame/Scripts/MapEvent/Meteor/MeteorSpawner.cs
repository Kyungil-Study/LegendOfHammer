using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeteorSpawner : MonoSingleton<MeteorSpawner>
{
    [SerializeField] private Meteor meteorPrefab; // 메테오 프리팹
    [SerializeField] private Vector2 BoundSize;
    [SerializeField] private Vector2 BoundOffset;
    [SerializeField,Tooltip("플레이어 중심 반지름")] private float spawnRadius;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)BoundOffset, BoundSize);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

       
    public void ExecuteMapEvent(int damage, int spawnCount)
    {
        if(BattlePopupSystem.Instance != null)
            BattlePopupSystem.Instance.MeteorAlarm.ExecuteAlarm();
        
        for (int i = 0; i < spawnCount; i++)
        {
            var randCircle = Random.insideUnitCircle * spawnRadius;
            var spawnPosition = Squad.Instance.transform.position + new Vector3(randCircle.x , randCircle.y , 0); 
            
            Meteor meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
            meteor.transform.SetParent(transform);
        }
       
    }
    
#if UNITY_EDITOR
    [PropertySpace(20),Title("Test Settings")]
    [SerializeField] int testSpawnCount = 5;
    [Button]
    public void TestSpawn()
    {
        if (EditorApplication.isPlaying == false)
            return;
        
        ExecuteMapEvent(0, testSpawnCount);
    }
#endif
}

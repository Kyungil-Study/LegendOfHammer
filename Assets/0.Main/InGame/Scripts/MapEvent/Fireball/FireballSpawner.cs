using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public enum FireballPointType
{
    Upper,
    Left,
}
public class FireballSpawner : MonoSingleton<FireballSpawner>
{
    [SerializeField] Transform upperSpawnPoint;
    [SerializeField] Transform leftSpawnPoint;

    [SerializeField] Fireball fireballPrefab;
    
    [LabelText("이벤트 트리거 게이지 기준"), Range(0,1), SerializeField] private float fireballTriggerLimit = 0.2f;

    [LabelText("이벤트 주기"), Range(0,1), SerializeField] private float fireballEventInterval = 0.5f;
    
    private IEnumerator SpawnFireballsCoroutine(int damage, IReadOnlyList<FireBallMapEvent.FireballSpawnJob> fireballSpawnJobs)
    {
        foreach (var fireballSpawnJob in fireballSpawnJobs)
        {
            yield return new WaitForSeconds(fireballSpawnJob.attackDelay);
            var spawnPosition = CalculatePosition(fireballSpawnJob.pointType);
            var spawnRotation = CalculateRotation(fireballSpawnJob.pointType);
            var fireball = Instantiate(fireballPrefab, spawnPosition, spawnRotation);
            fireball.Setup(damage);
        }
    }

    public Quaternion CalculateRotation(FireballPointType fireballPointType)
    {
        switch (fireballPointType)
        {
            case FireballPointType.Upper:
                return upperSpawnPoint.rotation; // 2D 상단은 회전 없음
            case FireballPointType.Left:
                return leftSpawnPoint.rotation; // 2D 왼쪽은 90도 회전
            default:
                throw new ArgumentOutOfRangeException(nameof(fireballPointType), fireballPointType, null);
            
        }
    }

    
    public Vector3 CalculatePosition( FireballPointType fireballPointType)
    { 
        var targetPosition = Squad.Instance.transform.position;
        switch (fireballPointType)
        {
            case FireballPointType.Upper:
            {
                if (upperSpawnPoint == null)
                {
                    throw new NullReferenceException("Upper spawn point is not assigned.");
                }
                return new Vector3(targetPosition.x , upperSpawnPoint.position.y, 0); // 2d
            }
            case FireballPointType.Left:
            {
                if (leftSpawnPoint == null)
                {
                    throw new NullReferenceException("Left spawn point is not assigned.");
                }
                return new Vector3(leftSpawnPoint.position.x , targetPosition.y, 0); // 2d
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(fireballPointType), fireballPointType, null);
        }
    }

    public void ExecuteMapEvent(int damage, IReadOnlyList< FireBallMapEvent.FireballSpawnJob> fireballSpawnJobs)
    {
        BattlePopupSystem.Instance.FireballAlarm.ExecuteAlarm();
        StartCoroutine(SpawnFireballsCoroutine(damage, fireballSpawnJobs));
    }
}

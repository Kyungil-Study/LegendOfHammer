using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[Serializable]
public enum FireballPointType
{
    Upper,
    Left,
    Right, // 현재 사용하지 않지만, 추후 확장성을 위해 남겨둠
    Bottom // 현재 사용하지 않지만, 추후 확장성을 위해 남겨둠
}
public class FireballSpawner : MonoSingleton<FireballSpawner>
{
    [SerializeField] Transform upperSpawnPoint;
    [SerializeField] Transform leftSpawnPoint;
    [SerializeField] Transform rightSpawnPoint; // 현재 사용하지 않지만, 추후 확장성을 위해 남겨둠
    [SerializeField] Transform bottomSpawnPoint; // 현재 사용하지 않지만, 추후 확장성을 위해 남겨둠
    
    [SerializeField] Fireball fireballPrefab;
    

    
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
            case FireballPointType.Right:
                return rightSpawnPoint.rotation; // 2D 오른쪽은 90도 회전
            case FireballPointType.Bottom:
                return bottomSpawnPoint.rotation; // 2D 하단은 회전 없음
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
            case FireballPointType.Right:
            {
                if (rightSpawnPoint == null)
                {
                    throw new NullReferenceException("Right spawn point is not assigned.");
                }
                return new Vector3(rightSpawnPoint.position.x , targetPosition.y, 0); // 2d
            }
            case FireballPointType.Bottom:
            {
                if (bottomSpawnPoint == null)
                {
                    throw new NullReferenceException("Bottom spawn point is not assigned.");
                }
                return new Vector3(targetPosition.x , bottomSpawnPoint.position.y, 0); // 2d
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(fireballPointType), fireballPointType, null);
        }
    }

    public void ExecuteMapEvent(int damage, IReadOnlyList< FireBallMapEvent.FireballSpawnJob> fireballSpawnJobs)
    {
        if(BattlePopupSystem.Instance != null)
            BattlePopupSystem.Instance.FireballAlarm.ExecuteAlarm();
        StartCoroutine(SpawnFireballsCoroutine(damage, fireballSpawnJobs));
    }
    
    [PropertySpace(20),Title("Test Settings")]
    [SerializeField] private FireBallMapEvent TestFireBallMapEvent;
    [Button] public void TestSpawn()
    {
        if (EditorApplication.isPlaying == false)
            return;
        
        ExecuteMapEvent(100, TestFireBallMapEvent.FireballSpawnJobs);
        foreach (var job in TestFireBallMapEvent.FireballSpawnJobs)
        {
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class FireballSpawner : MonoBehaviour
{
    [SerializeField] private LayerMask targetMask;
    [SerializeField] Transform upperSpawnPoint;
    [SerializeField] Transform leftSpawnPoint;

    [SerializeField] Fireball fireballPrefab;
    
    [LabelText("이벤트 트리거 게이지 기준"), Range(0,1), SerializeField] private float fireballTriggerLimit = 0.2f;

    [LabelText("이벤트 주기"), Range(0,1), SerializeField] private float fireballEventInterval = 0.5f;
    
    private void Awake()
    {
        BattleEventManager.RegistEvent<StartBattleEventArgs>(OnStartBattle);
        BattleManager.Instance.ChaseGuage.Events.OnValueChanged += OnChaseGuageValueChanged;
    }

    private void OnChaseGuageValueChanged(float arg1, float arg2)
    {
        var ratio = arg1 / arg2;
        if (ratio >= fireballTriggerLimit)
        {
            BattleManager.Instance.ChaseGuage.Events.OnValueChanged -= OnChaseGuageValueChanged;
            StartCoroutine(SpawnFireballsCoroutine());
        }
    }

    private IEnumerator SpawnFireballsCoroutine()
    {
        while (gameObject.activeInHierarchy)
        {
            Instantiate(fireballPrefab, leftSpawnPoint.position, leftSpawnPoint.rotation);
            yield return new WaitForSeconds(fireballEventInterval);
        }
    }
    
    public Vector3 CalculatePosition(FireballPointType fireballPointType)
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

    private void OnStartBattle(StartBattleEventArgs obj)
    {
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        //
    }
}

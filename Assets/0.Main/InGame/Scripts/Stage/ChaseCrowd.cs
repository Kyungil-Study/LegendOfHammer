using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ChaseCrowd : MonoBehaviour
{
    Vector3 originPosition;
    [SerializeField] private Transform destination; // 추적할 대상 위치
    [SerializeField] private float attackDelay = 0.5f; // 추적 공격 딜레이
    [SerializeField] private float chaseSpeed = 5f; // 추적 속도
    [Range(0,1), SerializeField] private float normalizedTriggerGauge = 0.3f; // 추적 게이지가 이 값 이상일 때 추적 시작
    [SerializeField] private float chaseAttackInterval = 1f; // 추적 공격 간격
    [SerializeField] bool isChasing = false;
    [SerializeField] private Ease chaseEase = Ease.Linear; // 추적 속도 계수
    private void Awake()
    {
        var callbacks = BattleEventManager.Instance.Callbacks;
        originPosition = transform.position;
        
        BattleManager.Instance.ChaseGuage.Events.OnValueChanged += OnChaseGuageValueChanged;
        
    }

    private void Start()
    {
        StartCoroutine(ChaseCoroutine());
    }


    private void OnChaseGuageValueChanged(float arg1, float arg2)
    {
        var ratio = arg1 / arg2;
        isChasing = ratio >= normalizedTriggerGauge;
    }

    private IEnumerator GoTo(Vector3 targetPosition)
    {
        yield return new WaitForSeconds(chaseSpeed);
    }

    private IEnumerator ChaseCoroutine()
    {
        while (gameObject.activeInHierarchy)
        {
            if (isChasing)
            {
                var gotoDestination = transform.DOMove( destination.position, chaseSpeed)
                    .SetEase(chaseEase);
                    //.OnComplete(() => Debug.Log($"Chased to {destination.name} at position {destination.position}"));
                yield return gotoDestination.WaitForCompletion();
                var gotoOrigin = transform.DOMove(originPosition, chaseSpeed).SetEase(Ease.Linear);
                yield return gotoOrigin.WaitForCompletion();
                yield return new WaitForSeconds(chaseAttackInterval);
            }

            yield return null;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IBattleCharacter character))
        {
            if (character is Squad squad)
            {
                // 영웅이 충돌했을 때 처리
                Debug.Log($"Hero {squad.name} has entered the crowd.");
            }
        }
    }
}

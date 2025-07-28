using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider2D))]
public class ChaseCrowd : MonoSingleton<ChaseCrowd>, IBattleCharacter
{
    Vector3 originPosition;
    [SerializeField] private Transform destination; // 추적할 대상 위치
    [SerializeField] private float attackDelay = 0.5f; // 추적 공격 딜레이
    [SerializeField] private float chaseSpeed = 5f; // 추적 속도
    [Range(0,1), SerializeField] private float normalizedTriggerGauge = 0.3f; // 추적 게이지가 이 값 이상일 때 추적 시작
    [SerializeField] private float chaseAttackInterval = 1f; // 추적 공격 간격
    [SerializeField] bool isChasing = false;
    [SerializeField] private Ease chaseEase = Ease.Linear; // 추적 속도 계수
    
    [FormerlySerializedAs("attackSignal")] [SerializeField] private AttackAlertSignal attackAlertSignal; // 추적 공격 시그널
    [SerializeField] private BoxCollider2D attackCollider; // 추적 공격 콜라이더
    
    [SerializeField] private float attackPower = 10f; // 추적 공격력
    
    private void Awake()
    {
        originPosition = transform.position;
        attackCollider = GetComponent<BoxCollider2D>();
        attackCollider.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(ChaseCoroutine());
    }


    private IEnumerator ChaseCoroutine()
    {
        transform.position = originPosition;
        attackAlertSignal.gameObject.SetActive(true);
        yield return new WaitForSeconds(attackDelay);
        attackCollider.enabled = true;
        var gotoDestination = transform.DOMove(destination.position, chaseSpeed)
            .SetEase(chaseEase);
        //.OnComplete(() => Debug.Log($"Chased to {destination.name} at position {destination.position}"));
        yield return gotoDestination.WaitForCompletion();
        attackAlertSignal.gameObject.SetActive(false);
        var gotoOrigin = transform.DOMove(originPosition, chaseSpeed).SetEase(Ease.Linear);
        yield return gotoOrigin.WaitForCompletion();
        attackCollider.enabled = false;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IBattleCharacter character))
        {
            if (character is Squad squad)
            {
                // 영웅이 충돌했을 때 처리
                Debug.Log($"Hero {squad.name} has entered the crowd.");
                TakeDamageEventArgs takeDamageArgs = new TakeDamageEventArgs(this, squad, Mathf.RoundToInt(attackPower));
                BattleEventManager.CallEvent(takeDamageArgs);
            }
        }
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        // 추적 크라우드가 피해를 받았을 때 처리
    }

    public void ExecuteMapEvent( int damage)
    {
        attackPower = damage;
        StopCoroutine(ChaseCoroutine());
        StartCoroutine(ChaseCoroutine());
    }
}

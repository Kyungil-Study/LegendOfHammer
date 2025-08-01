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
    
    [SerializeField] private Ease chaseEase = Ease.Linear; // 추적 속도 계수
    
    [FormerlySerializedAs("attackSignal")] [SerializeField] private AttackAlertSignal attackAlertSignal; // 추적 공격 시그널
    [SerializeField] private BoxCollider2D attackCollider; // 추적 공격 콜라이더
    
    [SerializeField] private float attackPower = 10f; // 추적 공격력
    
    protected override void Awake()
    {
        base.Awake();
        originPosition = transform.position;
        attackCollider = GetComponent<BoxCollider2D>();
        attackCollider.enabled = false;
    }

    private void Start()
    {
        attackPower = 0; // 연출을 위한 초기 공격력 설정
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
                TakeDamageEventArgs takeDamageArgs = new TakeDamageEventArgs(
                    this, squad, DamageType.Enemy, Mathf.RoundToInt(attackPower));
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
        BattlePopupSystem.Instance.ChaserAlarm.ExecuteAlarm();
        attackPower = damage;
        StopCoroutine(ChaseCoroutine());
        StartCoroutine(ChaseCoroutine());
    }
}

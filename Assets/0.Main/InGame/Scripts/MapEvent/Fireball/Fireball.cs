using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;


public class Fireball : MonoBehaviour , IBattleCharacter
{
    private static readonly int EXPLODE = Animator.StringToHash("Explode");
    [SerializeField] Collider2D attackCollider; // 공격 범위 콜라이더
    [SerializeField] private GameObject attackArea; // 타겟 레이어 마스크
    [SerializeField] private Transform endPoint; // 공격 종료 지점
    [SerializeField] private float attackDelay = 0.5f; // 공격 딜레이
    [SerializeField] Animator fireballAnimator; // 파이어볼 애니메이션
    [SerializeField] int damage = 10; // 예시로 10 데미지
    [SerializeField] private float moveDuration; // 타겟 레이어 마스크
    [SerializeField] float destroyDelay = 2f; // 애니메이션 재생 후 파괴될 시간
    TweenerCore<Vector3, Vector3, VectorOptions> moveTween;
    Coroutine moveCoroutine;
    private void Start()
    {
        moveCoroutine = StartCoroutine(MoveCoroutine());
    }
    
    public void Setup(int damage)
    {
        this.damage = damage;
    }

    IEnumerator MoveCoroutine()
    {
        var destination = endPoint.position;
        yield return new WaitForSeconds(attackDelay);
        attackArea.gameObject.SetActive(false);
        attackCollider.enabled = true; // 공격 범위 콜라이더 활성화
        moveTween = transform.DOMove(destination, moveDuration);
        yield return moveTween.WaitForCompletion();
        Explode();
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        attackCollider.enabled = false; // 공격 범위 콜라이더 비활성화 중복 피격 방지용
        if (other.TryGetComponent<Squad>(out var target))
        {
            target.TakeDamage(new TakeDamageEventArgs(this, target, DamageType.Enemy, damage));
        }
        StopCoroutine(moveCoroutine);
        Explode();
    }

    private void Explode()
    {
        if (moveTween != null)
        {
            moveTween.Kill();
        }
        fireballAnimator.SetTrigger(EXPLODE);
        Destroy(gameObject, destroyDelay);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class Meteor : MonoBehaviour , IBattleCharacter
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float fallDuration = 2f;
    [SerializeField] private float attackDuration = 1f; // 공격 지속 시간
    
    [SerializeField] private AnimationClip explosionClip;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject fall;
    
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    
    PolygonCollider2D attackCollider;
    bool isHitted = false;
    Animator explosionAnimator;

    void Start()
    {
        explosionAnimator = explosion.GetComponent<Animator>();
        attackCollider = GetComponent<PolygonCollider2D>();
        fall.transform.position = startPoint.position;
        StartCoroutine(FallMeteorCoroutine());
    }

    IEnumerator FallMeteorCoroutine()
    {
        fall.transform.position = startPoint.position;
        fall.gameObject.SetActive(true);
        explosion.gameObject.SetActive(false);

        var tween = fall.transform.DOMove(endPoint.position, fallDuration)
            .SetEase(Ease.InQuad).OnStepComplete(
                () =>
                {
                    fall.gameObject.SetActive(false);
                    explosion.gameObject.SetActive(true);
                }
            ).SetLink(gameObject);
        yield return tween.WaitForCompletion();
        
        // 폭발하는 동안 공격 콜라이더 활성화
        attackCollider.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        attackCollider.enabled = false;
        
        yield return new WaitUntil(() =>
        {
            return explosionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        });
        
        Debug.Log($"Meteor Attack Finished {endPoint.position}");
        Destroy(gameObject);
        
    }
    
    
    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isHitted) return;
        
        if(other.TryGetComponent(out Squad squad))
        {
            isHitted = true;
            Debug.Log($"Meteor Hit {squad.name}");
            squad.TakeDamage(new TakeDamageEventArgs(this, squad, DamageType.Enemy, damage));
        }
    }
}

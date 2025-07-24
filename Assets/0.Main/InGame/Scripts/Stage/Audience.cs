using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class Audience : MonoBehaviour
{
    [SerializeField] private AnimationClip[] audienceAnimation;
    [SerializeField] private float ScaleMin = 3f;
    [SerializeField] private float ScaleMax = 5f;
    [SerializeField] private int SortingOrderMin = 1000; // 초기화 시간
    [SerializeField] private int SortingOrderMax = 2000; // 초기화 시간
    
    private float normalizedInitTime = 0f;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public void Setup(AudienceSpawner spawner)
    {
        transform.localScale = Vector3.one * Random.Range(ScaleMin, ScaleMax);
        
        
        var AudienceAnimations = spawner.AudienceAnimations;
        animator = GetComponent<Animator>();
        
        var clipIndex = Random.Range(0, AudienceAnimations.Length);
        //Debug.Log($"clip Index : " + clipIndex);
        var clip = AudienceAnimations[clipIndex];
        spawner.AudienceAOCMap.TryGetValue(clip, out var animatorOverrideController);
        animator.runtimeAnimatorController = animatorOverrideController;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        var sortingOrder = Random.Range(SortingOrderMin, SortingOrderMax);
        //Debug.Log($"Sorting Order : {sortingOrder}");
        spriteRenderer.sortingOrder = sortingOrder;
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
    }
}

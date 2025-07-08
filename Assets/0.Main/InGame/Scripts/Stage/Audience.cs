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

    public void Setup()
    {
        transform.localScale = Vector3.one * Random.Range(ScaleMin, ScaleMax);
        
        
        
        animator = GetComponent<Animator>();
        
        var animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animatorOverrideController.name = "AudienceOverrideController";
        var clipIndex = Random.Range(0, audienceAnimation.Length);
        Debug.Log($"clip Index : " + clipIndex);
        
        animatorOverrideController["Audience_Idle"] = audienceAnimation[clipIndex];
        animator.runtimeAnimatorController = animatorOverrideController;
        animator.Play("Audience_Idle", 0, Random.Range(0f, 1f));
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        var sortingOrder = Random.Range(SortingOrderMin, SortingOrderMax);
        Debug.Log($"Sorting Order : {sortingOrder}");
        spriteRenderer.sortingOrder = sortingOrder;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audience : MonoBehaviour
{
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Animator audienceAnimator;
    [SerializeField] private SpriteRenderer audienceSpriteRenderer;
    [SerializeField] private AnimationClip[] audienceAnimation;
    [SerializeField] private float ScaleMin = 3f;
    [SerializeField] private float ScaleMax = 5f;
    [SerializeField] private int SortingOrderMin = 1000; // 초기화 시간
    [SerializeField] private int SortingOrderMax = 2000; // 초기화 시간
    

    public void Setup(AudienceSpawner spawner, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        modelTransform.localScale = Vector3.one * Random.Range(ScaleMin, ScaleMax);
        transform.position = spawnPosition;
        modelTransform.rotation = spawnRotation;
        
        var AudienceAnimations = spawner.AudienceAnimations;
        
        var clipIndex = Random.Range(0, AudienceAnimations.Length);
        //Debug.Log($"clip Index : " + clipIndex);
        var clip = AudienceAnimations[clipIndex];
        spawner.AudienceAOCMap.TryGetValue(clip, out var animatorOverrideController);
        audienceAnimator.runtimeAnimatorController = animatorOverrideController;
        
        var sortingOrder = Random.Range(SortingOrderMin, SortingOrderMax);
        //Debug.Log($"Sorting Order : {sortingOrder}");
        audienceSpriteRenderer.sortingOrder = sortingOrder;
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
    }
}

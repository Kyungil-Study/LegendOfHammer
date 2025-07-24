using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    [Tooltip("이펙트 크기 조절")]
    [SerializeField] private float effectScaleMultiplier = 1f;

    private Animator animator;

    private void Awake()
    {
        transform.localScale *= effectScaleMultiplier;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (animator.runtimeAnimatorController != null)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                float duration = clips[0].length;
                Destroy(gameObject, duration);
                return;
            }
        }
        
        Destroy(gameObject);
    }
}

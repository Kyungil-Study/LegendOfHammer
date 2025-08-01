using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PunchEffctor : MonoBehaviour
{
    [SerializeField] private float TweenScaleMin = 1;
    [SerializeField] private float TweenScaleMax = 1.5f;
    [SerializeField] private float TweenDurationMin = 0.5f;
    [SerializeField] private float TweenDurationMax = 1f;
    
    Vector3 originalScale = Vector3.one;
    Tweener tweener;
    public void PlayEffect()
    {
        var punchScale = UnityEngine.Random.Range(TweenScaleMin, TweenScaleMin);
        
        if (tweener != null)
        {
            tweener.Kill();
            transform.localScale = originalScale; 
            tweener = null;
        }
        
        originalScale = transform.localScale;
        
        tweener =transform.DOPunchScale(
            new Vector3(punchScale, punchScale, punchScale),
            UnityEngine.Random.Range(TweenDurationMin, TweenDurationMax),
            vibrato: 10,
            elasticity: 0.1f
        );
    }

    private void OnDisable()
    {
        if (tweener != null)
        {
            tweener.Kill();
            tweener = null;
        }
    }
}

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
        var punchScale = UnityEngine.Random.Range(TweenScaleMin, TweenScaleMax);
        
        if (tweener != null && tweener.IsActive())
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
        ).SetLink(gameObject).OnKill(() =>
            {
                Debug.Log("Punch Effect finished");
            }
            );
    }

    private void OnDestroy()
    {
        if (tweener != null&& tweener.IsActive())
        {
            tweener.Kill();
            tweener = null;
        }
    }

    private void OnDisable()
    {
        if (tweener != null&& tweener.IsActive())
        {
            Debug.Log("Punch Effect Kill on Disable");
            tweener.Kill();
            tweener = null;
        }
    }
}

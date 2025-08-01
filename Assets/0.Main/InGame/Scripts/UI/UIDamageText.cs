using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UIDamageText : MonoBehaviour
{
    [SerializeField] private float TweenScaleMin = 1;
    [SerializeField] private float TweenScaleMax = 1.5f;
    [SerializeField] private float TweenDurationMin = 0.5f;
    [SerializeField] private float TweenDurationMax = 1f;
    
    Tweener tweener;
    
    Vector3 originalScale;
    [Required, SerializeField] private TMP_Text damageTxt;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        damageTxt.enabled  = true;  
        transform.localScale = originalScale; // Reset scale when enabled
        var punchScale = UnityEngine.Random.Range(TweenScaleMin, TweenScaleMax);
        
        tweener =transform.DOPunchScale(
            new Vector3(punchScale, punchScale, punchScale),
            UnityEngine.Random.Range(TweenDurationMin, TweenDurationMax),
            vibrato: 10,
            elasticity: 0.1f
        );
        
        BattleEventManager.RegistEvent<PauseBattleEventArgs>(OnPauseBattle);
    }

    private void OnPauseBattle(PauseBattleEventArgs args)
    {
        damageTxt.enabled = !args.IsPaused;
    }

    private void OnDisable()
    {
        if (tweener != null)
        {
            tweener.Kill();
            tweener = null;
            
        }
        BattleEventManager.UnregistEvent<PauseBattleEventArgs>(OnPauseBattle);
    }
}

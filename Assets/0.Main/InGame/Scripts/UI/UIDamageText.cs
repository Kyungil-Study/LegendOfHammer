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
        BattleEventManager.RegistEvent<PauseBattleEventArgs>(OnPauseBattle);
    }

    private void OnEnable()
    {
        try
        {
            if (tweener != null && tweener.IsActive())
            {
                tweener.Kill();
            }
            
            damageTxt.enabled  = true;  
            transform.localScale = originalScale; // Reset scale when enabled
            var punchScale = UnityEngine.Random.Range(TweenScaleMin, TweenScaleMax);
        
            tweener =transform.DOPunchScale(
                new Vector3(punchScale, punchScale, punchScale),
                UnityEngine.Random.Range(TweenDurationMin, TweenDurationMax),
                vibrato: 10,
                elasticity: 0.1f
            ).SetLink(gameObject);
        }
        catch (Exception e)
        {
            Debug.Log($"Error while enabling UIDamageText: " + e.Message );
            Console.WriteLine(e);
            throw;
        }
       
        
    }

    private void OnPauseBattle(PauseBattleEventArgs args)
    {
        damageTxt.enabled = !args.IsPaused;
    }

    private void OnDestroy()
    {
        try
        {
            if (tweener != null && tweener.IsActive())
            {
                tweener.Kill();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning( "Error while disabling UIDamageText: " + e.Message);
            throw;
        }

    }

    private void OnDisable()
    {
        try
        {
            if (tweener != null && tweener.IsActive())
            {
                tweener.Kill();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning( "Error while disabling UIDamageText: " + e.Message);
            throw;
        }
        
    }
}

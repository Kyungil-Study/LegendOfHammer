using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageCinematic : MonoBehaviour
{
    [Header("Cinematic Settings")]
    [SerializeField] Animator cinematicPanelAnimator;
    private void OnEnable()
    {
        StartCoroutine(PlayCinematic());
    }
    
    private void OnDisable()
    {
    }
    
    IEnumerator PlayCinematic()
    {
        cinematicPanelAnimator.Play("Upgrade_Video");
        yield return new WaitUntil(() =>
        {
            return cinematicPanelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        });
        gameObject.SetActive(false);
    }
}

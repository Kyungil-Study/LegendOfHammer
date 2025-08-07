using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoSingleton<BattleUIController> ,IPageFlowManageable
{
    [SerializeField] private UIPage[] pages;
    
    Dictionary<UIPageType, UIPage> pagesDict = new Dictionary<UIPageType, UIPage>();
    UIPage currentPage;
    protected override void Initialize()
    {
        base.Initialize();
        foreach (var page in pages)
        {
            page.Setup(this);
            page.gameObject.SetActive(false);
            
            pagesDict[page.UIPageType] = page;
        }
    }

    public void SwapPage(UIPageType nextPageType)
    {
        Debug.Log($"SwapPage called with type: {nextPageType}");
        if (currentPage != null)
        {
            currentPage.Exit();
            currentPage.gameObject.SetActive(false);
        }
        if (pagesDict.TryGetValue(nextPageType, out var nextPage))
        {
            currentPage = nextPage;
            currentPage.gameObject.SetActive(true);
            currentPage.Enter();
        }
        else
        {
            Debug.LogError($"Page of type {nextPageType} not found.");
        }
    }

}

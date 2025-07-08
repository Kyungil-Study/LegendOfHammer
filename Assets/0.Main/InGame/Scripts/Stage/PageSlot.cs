using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageSlot : MonoBehaviour
{
    public List<StagePage> stagePages = new List<StagePage>();
    int currentPageIndex = -1; // 현재 페이지 인덱스

    public void AddPage(StagePage page)
    {
        page.transform.SetParent(transform);
        stagePages.Add(page);
        page.gameObject.SetActive(false);
    }
    
    public void NextPage()
    {
        if(currentPageIndex >= 0)
            stagePages[currentPageIndex].gameObject.SetActive(false);
        currentPageIndex++;
        stagePages[currentPageIndex].gameObject.SetActive(true);
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PageScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1.0f;
    [SerializeField] private float scrollDistance = 10.0f;
    [SerializeField] private PageSlot[] pages;
    public PageSlot[] Pages => pages;
    bool[] reserveNextPage;

    private void Awake()
    {
        reserveNextPage = Enumerable.Repeat(false, pages.Length).ToArray(); 
        BattleEventManager.Instance.Callbacks.OnNextPage += OnNextPage;
    }

    private void OnNextPage(NextPageEventArgs obj)
    {
        for (int i = 0; i < reserveNextPage.Length; i++)
        {
            reserveNextPage[i] = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            var page = pages[i];
            
            var topPosY = page.transform.position.y + scrollDistance;
            if (topPosY <= -scrollDistance)
            {
                page.transform.position = new Vector3(
                    page.transform.position.x,
                    page.transform.position.y + scrollDistance*2*2,
                    0);
                
                if(reserveNextPage[i])
                {
                    page.NextPage();
                    reserveNextPage[i] = false;
                }
            }
                
            page.transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
        }
     
        
    }
}

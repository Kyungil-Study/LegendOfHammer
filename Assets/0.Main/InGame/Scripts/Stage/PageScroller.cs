using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class PageScroller : MonoBehaviour
{
    
    [SerializeField] private float nextPageInterval = 60f; // Time in seconds to show next page
    [Space(10), SerializeField] private PageSlot[] pages;
    [Space(10),SerializeField] StagePage[] stagePagePrefabs;
    [SerializeField] CinemachineCamera cinemachineVirtualCamera;

    [SerializeField] MapSetting mapSetting;
    public PageSlot[] Pages => pages;
    bool[] reserveNextPage;
    
    bool isStarted = false;

    private void Awake()
    {
        reserveNextPage = Enumerable.Repeat(false, pages.Length).ToArray();
        BattleEventManager.RegistEvent<ReadyBattleEventArgs>(OnReadyBattle);
        BattleEventManager.RegistEvent<StartBattleEventArgs>(OnStartBattle);
        BattleEventManager.RegistEvent<EndBattleEventArgs>(OnEndBattle);
        viewHeight = cinemachineVirtualCamera.Lens.OrthographicSize * 2f;
    }

    private void OnEndBattle(EndBattleEventArgs obj)
    {
        isStarted = false;
    }

    private void OnReadyBattle(ReadyBattleEventArgs args)
    {
        int pageIndex = (args.StageIndex % stagePagePrefabs.Length);
        int NextPageIndex = (pageIndex + 1) % stagePagePrefabs.Length;

        foreach (var slot in Pages)
        {
            var page = Instantiate(stagePagePrefabs[pageIndex], slot.transform);
            slot.AddPage(page);
            var nextPage = Instantiate(stagePagePrefabs[NextPageIndex], slot.transform);
            slot.AddPage(nextPage);
            slot.NextPage();
        }
    }

    private void OnStartBattle(StartBattleEventArgs args)
    {
        isStarted = true;
        StartCoroutine(NextPageTimer(nextPageInterval));
    }

    public int startIndex;
    public int endIndex;
    
    float viewHeight;
    
    private IEnumerator NextPageTimer(float time)
    {
        yield return new WaitForSeconds(time);
        for (int i = 0; i < reserveNextPage.Length; i++)
        {
            reserveNextPage[i] = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStarted)
            return;
        
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * (mapSetting.ScrollSpeed * Time.deltaTime);
        transform.position = curPos + nextPos;

        var endPage = pages[endIndex];
        var endPageTransform = endPage.transform;
        if (endPageTransform.position.y < -viewHeight)
        {
            Vector3 backSpritePos = pages[startIndex].transform.localPosition;
            Vector3 frontSpritePos = endPageTransform.localPosition;
            endPageTransform.localPosition
                = backSpritePos + Vector3.up * (viewHeight);
            
            if(reserveNextPage[endIndex])
            {
                endPage.NextPage();
                reserveNextPage[endIndex] = false;
            }
            
            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave-1 == -1) ? pages.Length - 1 : startIndexSave - 1;
        }
    }
}

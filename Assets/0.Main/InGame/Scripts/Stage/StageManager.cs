using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class StageManager : MonoBehaviour
{
    [Header("Stage 시간 세팅")]
    [SerializeField] private float nextPageInterval = 60f; // Time in seconds to show next page
    
    [SerializeField] PageScroller pageScroller;
    [SerializeField] StagePage[] stagePagePrefabs;
    
    private float stageStartTime = 0f;
    Queue<StageWave> stageWavesQueue = new Queue<StageWave>();

    private void Awake()
    {
        BattleEventManager.Instance.Callbacks.OnStartBattle += StartGame;
        BattleEventManager.Instance.Callbacks.OnEndBattle += EndGame;
    }

    private void EndGame(EndBattleEventArgs args)
    {
        pageScroller.enabled = false;
    }

    
    private void StartGame(StartBattleEventArgs startEventArgs)
    {
        var stageWaves = StageDataManager.Instance.Records;
        foreach (var wave in stageWaves)
        {
            stageWavesQueue.Enqueue(wave);
        }
        
        pageScroller.enabled = true;
        int pageIndex = (startEventArgs.StageIndex % stagePagePrefabs.Length);
        int NextPageIndex = (pageIndex + 1) % stagePagePrefabs.Length;

        var pageSlots = pageScroller.Pages;
        foreach (var slot in pageSlots)
        {
            var page = Instantiate(stagePagePrefabs[pageIndex], slot.transform);
            slot.AddPage(page);
            var nextPage = Instantiate(stagePagePrefabs[NextPageIndex], slot.transform);
            slot.AddPage(nextPage);
            
            slot.NextPage();
        }
        
        stageStartTime = Time.time;
        StartCoroutine(NextPageTimer(nextPageInterval));
        StartCoroutine(UpdateWave_Coroutine());
    }

    private IEnumerator NextPageTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Next page triggered.");
        NextPageEventArgs args = new NextPageEventArgs();
        BattleEventManager.Instance.CallEvent(args);
    }

    private IEnumerator UpdateWave_Coroutine()
    {
        while (stageWavesQueue.Count > 0)
        {
            yield return new WaitUntil(() => 
            {
                StageWave currentWave = stageWavesQueue.Peek();
                return (Time.time - stageStartTime) >= currentWave.PlayTime;
            });
            StageWave currentWave = stageWavesQueue.Dequeue();
            if (currentWave.WaveEmerge)
            {
                Debug.Log($"Wave emerge: {currentWave.WaveRank}");
                MonsterSpawner.Instance.SpawnMonster(currentWave.WaveRank);
                
            }
            
            if (currentWave.ObstacleEmerge)
            {
                Debug.Log($"Obstacles emerge: {currentWave.ObstacleEmerge}");
            }

            if (currentWave.MapEventEmerge)
            {
                Debug.Log($"Map events emerge: {currentWave.MapEventEmerge}");
            }
            
        }
    }
}

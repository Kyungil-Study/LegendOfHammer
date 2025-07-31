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
    
    private float stageStartTime = 0f;
    Queue<StageWave> stageWavesQueue = new Queue<StageWave>();

    private void Awake()
    {
        BattleEventManager.RegistEvent<StartBattleEventArgs>(StartGame);
        BattleEventManager.RegistEvent<EndBattleEventArgs>(EndGame);
    }

    private void EndGame(EndBattleEventArgs args)
    {
    }

    
    private void StartGame(StartBattleEventArgs startEventArgs)
    {
        var stageWaves = StageDataManager.Instance.Records;
        foreach (var wave in stageWaves)
        {
            stageWavesQueue.Enqueue(wave);
        }
        stageStartTime = Time.time;
        StartCoroutine(UpdateWave_Coroutine());
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MapEventManager : MonoBehaviour
{
    [SerializeField] MapEventTableSAO mapEventTable; // 맵 이벤트 테이블
    IReadOnlyList<MapEventPatternSAO> filteredMapEvents; // 필터링된 맵 이벤트 테이블
    
    [Range(0,1), SerializeField] private float startMapEvent = 0.3f; // 추적 게이지가 이 값 이상일 때 맵이벤트 시작

    [SerializeField] private float mapEventInterval = 15f; // 맵 이벤트 간격
    private void Awake()
    {
        BattleEventManager.RegistEvent<StartBattleEventArgs>( StartBattle);
        BattleManager.Instance.ChaseGuage.Events.OnValueChanged += OnChaseGuageValueChanged;

    }

    void StartBattle(StartBattleEventArgs args)
    {
        filteredMapEvents = mapEventTable.FiltertedMapEventPatterns(args.StageIndex);
        StartCoroutine(SimulateMapEvents());
    }


    private void OnChaseGuageValueChanged(float arg1, float arg2)
    {
        var ratio = arg1 / arg2;
        if (ratio >= startMapEvent)
        {
            // 추적 게이지가 시작 맵 이벤트 값 이상일 때
            Debug.Log("Chase Gauge reached the threshold for map events.");
            BattleManager.Instance.ChaseGuage.Events.OnValueChanged -= OnChaseGuageValueChanged;
            //StartCoroutine(SimulateMapEvents());
        }
    }

    private IEnumerator SimulateMapEvents()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(mapEventInterval); // 다음 이벤트까지 대기
            int randomEventIndex = UnityEngine.Random.Range(0, filteredMapEvents.Count);
            filteredMapEvents[randomEventIndex].ExecuteEvent();
        }
    }
}


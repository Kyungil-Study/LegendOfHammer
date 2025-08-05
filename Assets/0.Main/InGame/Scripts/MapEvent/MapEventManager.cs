using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class MapEventTrigger
{
    [SerializeField] private float triggerTime; // 이벤트 발생 시간
    [SerializeField] private float mapEventInterval; // 이벤트 지속 시간
    public float TriggerTime => triggerTime;
    public float MapEventInterval => mapEventInterval;
}

public class MapEventManager : MonoBehaviour
{
    [SerializeField] List<MapEventTrigger> mapEventTriggers = new List<MapEventTrigger>();
    
    [SerializeField] MapEventTableSAO mapEventTable; // 맵 이벤트 테이블
    IReadOnlyList<MapEventPatternSAO> filteredMapEvents; // 필터링된 맵 이벤트 테이블
    
    [SerializeField] private float mapEventInterval = 15f; // 맵 이벤트 간격
    
    Coroutine mapEventCoroutine;
    private void Awake()
    {
        BattleEventManager.RegistEvent<StartBattleEventArgs>( StartBattle);
        BattleEventManager.RegistEvent<ReviveEventArgs>(Revive);
        BattleManager.Instance.ChaseGuage.Events.OnValueChanged += OnChaseGuageValueChanged;
        mapEventTriggers.Sort( (a, b) => a.TriggerTime.CompareTo(b.TriggerTime));
    }

    void StartBattle(StartBattleEventArgs args)
    {
        filteredMapEvents = mapEventTable.FiltertedMapEventPatterns(args.StageIndex);
        mapEventCoroutine = StartCoroutine(SimulateMapEvents());
    }

    public void Revive(ReviveEventArgs args)
    {
        filteredMapEvents = mapEventTable.FiltertedMapEventPatterns(BattleManager.Instance.StageIndex);
        StopCoroutine(mapEventCoroutine);
        mapEventCoroutine = StartCoroutine(SimulateMapEvents());
    }
    


    private void OnChaseGuageValueChanged(float arg1, float arg2)
    {
        var max = BattleManager.Instance.ChaseGuage.Max;
        var ratio = arg2 / max;
        if (mapEventTriggers.Count > 0)
        {
            Debug.Log($" Chase Gauge Ratio: {arg2}/{max} {ratio}, Trigger Time: {mapEventTriggers[0].TriggerTime}");
            if (ratio >= mapEventTriggers[0].TriggerTime)
            {
                // 추적 게이지가 시작 맵 이벤트 값 이상일 때
                Debug.Log("Chase Gauge reached the threshold for map events.");
                mapEventInterval = mapEventTriggers[0].MapEventInterval;
                int randomEventIndex = UnityEngine.Random.Range(0, filteredMapEvents.Count);
                filteredMapEvents[randomEventIndex].ExecuteEvent();
                
                // 주기 초기화
                StopCoroutine(mapEventCoroutine);
                mapEventCoroutine = StartCoroutine(SimulateMapEvents());
                
                // 활성화된 트리거 제거 
                mapEventTriggers.RemoveAt(0);
            }
            
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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapEventType
{
    ChaseCrowd, // 추격 크라우드
    Fireball, // 파이어볼
    Meteor, // 메테오
    Count
    // 다른 이벤트 타입 추가 가능
}

public class MapEventManager : MonoBehaviour
{
    [SerializeField] ChaseCrowd chaseCrowd; // 추격 크라우드
    [SerializeField] MeteorSpawner meteorSpawner; // 메테오 스포너
    [Range(0,1), SerializeField] private float startMapEvent = 0.3f; // 추적 게이지가 이 값 이상일 때 맵이벤트 시작
    [SerializeField] private MapEventType mapEventType = MapEventType.ChaseCrowd; // 맵 이벤트 타입

    [SerializeField] private float mapEventInterval = 15f; // 맵 이벤트 간격
    private void Awake()
    {
        BattleManager.Instance.ChaseGuage.Events.OnValueChanged += OnChaseGuageValueChanged;
    }

    private void Start()
    {
        //StartCoroutine(SimulateMapEvents());
    }

    private void OnChaseGuageValueChanged(float arg1, float arg2)
    {
        var ratio = arg1 / arg2;
        if (ratio >= startMapEvent)
        {
            Debug.Log($"MapEventManager: Starting map event of type {mapEventType} with normalizedTrigger: {startMapEvent}");
            BattleManager.Instance.ChaseGuage.Events.OnValueChanged -= OnChaseGuageValueChanged;
            StartCoroutine(SimulateMapEvents());
        }
    }

    private IEnumerator SimulateMapEvents()
    {
        while (gameObject.activeInHierarchy)
        {
            var eventCount = (int)MapEventType.Count;
            int randomEventIndex = UnityEngine.Random.Range(0, (int)MapEventType.Count);
            mapEventType = (MapEventType)randomEventIndex;

            switch (mapEventType)
            {
                case MapEventType.ChaseCrowd:
                    Debug.Log("MapEventManager: Starting ChaseCrowd event");
                    chaseCrowd.ExecuteMapEvent();
                    break;
                case MapEventType.Meteor:
                    Debug.Log("MapEventManager: Starting Meteor event");
                    meteorSpawner.ExecuteMapEvent();
                    break;
                case MapEventType.Fireball:
                    // Fireball 이벤트 처리 로직 추가
                    Debug.Log("MapEventManager: Fireball event not implemented yet");
                    break;
                default:
                    Debug.LogWarning($"MapEventManager: Unknown event type {mapEventType}");
                    break;
            }
            
            yield return new WaitForSeconds(mapEventInterval); // 다음 이벤트까지 대기
        }
        
    }
}


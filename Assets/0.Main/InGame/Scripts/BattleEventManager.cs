using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventManager : MonoSingleton<BattleEventManager>
{
    private static Dictionary<Type, Delegate> eventTable = new Dictionary<Type, Delegate>();

    protected override void Initialize()
    {
        base.Initialize();
        eventTable.Clear();
    }

    /// <summary>
    /// 이벤트 등록
    /// </summary>
    public static void RegistEvent<T>(Action<T> listener) where T : BattleEventArgs
    {
        if (Instance == null)
        {
            Debug.LogWarning("[BattleEventManager] Instance is null");
            return;
        }

        var eventType = typeof(T);
        if (eventTable.TryGetValue(eventType, out var existingDelegate))
        {
            eventTable[eventType] = Delegate.Combine(existingDelegate, listener);
            Debug.Log($"[BattleEventManager] Registered event listener for {eventType.Name}");
        }
        else
        {
            eventTable[eventType] = listener;
            Debug.Log($"[BattleEventManager] Created new event listener for {eventType.Name}");
        }
    }

    /// <summary>
    /// 이벤트 해제
    /// </summary>
    public static void UnregistEvent<T>(Action<T> listener) where T : BattleEventArgs
    {
        if (Instance == null)
        {
            Debug.LogWarning("[BattleEventManager] Instance is null");
            return;
        }

        var eventType = typeof(T);
        if (eventTable.TryGetValue(eventType, out var existingDelegate))
        {
            var current = Delegate.Remove(existingDelegate, listener);
            if (current == null)
            {
                eventTable.Remove(eventType);
                Debug.Log($"[BattleEventManager] Unregistered all listeners for {eventType.Name}");
            }
            else
            {
                eventTable[eventType] = current;
                Debug.Log($"[BattleEventManager] Unregistered listener for {eventType.Name}");
            }
        }
    }

    /// <summary>
    /// generic 특수화 : 데미지 가하는 이벤트의 경우 타겟에게만 호출되어야하는 함수가 정해져 있음  
    /// </summary>
    public static void CallEvent(TakeDamageEventArgs eventArgs)
    {
        eventArgs.Target.TakeDamage(eventArgs);
        BroadCastToListeners(eventArgs);
    }

    /// <summary>
    /// 이벤트 호출
    /// </summary>
    public static void CallEvent<T>(T eventArgs) where T : BattleEventArgs
    {
        BroadCastToListeners(eventArgs);
    }

    static void BroadCastToListeners<T>(T eventArgs) where T : BattleEventArgs
    {
        if (Instance == null)
        {
            Debug.LogWarning("[BattleEventManager] Instance is null");
            return;
        }

        var eventType = eventArgs.GetType();
        Debug.Log($"[BattleEventManager] Calling event: {eventType.Name} with args: {eventArgs}");

#if UNITY_EDITOR
        Debug.Log($"[BattleEventManager] Dispatching event: {eventType.Name}");
#endif

        if (eventTable.TryGetValue(eventType, out var del))
        {
            try
            {
                del.DynamicInvoke(eventArgs);
                Debug.Log($"[BattleEventManager] Successfully invoked event: {eventType.Name}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[BattleEventManager] Error invoking event {eventType.Name}: {ex}");
            }
        }
        else
        {
            Debug.LogWarning($"[BattleEventManager] No listeners registered for {eventType.Name}");
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool applicationIsQuitting = false;
    private bool isInitialized = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning($"[MonoSingleton] {typeof(T).Name} 인스턴스가 이미 종료되었습니다. 더 이상 접근할 수 없습니다.");
                return null;
            }
            
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>(true);
                if (_instance == null)
                {
                    Debug.LogError($"[MonoSingleton] {typeof(T).Name} 인스턴스가 존재하지 않습니다. 씬에 추가해주세요.");
                }
                else
                {
                    (_instance as MonoSingleton<T>).Initialize(); // Ensure the instance is of type MonoSingleton<T>
                }
                
            }
            return _instance;
        }
    }

    protected virtual void OnDisable()
    {
        if (_instance != null && _instance == this)
        {
            _instance = null; // Clear the instance when this MonoSingleton is disabled
        }
    }


    protected virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    protected virtual void Initialize()
    {
        isInitialized = true;
    }
    
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            Initialize();
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[MonoSingleton] {typeof(T).Name} 인스턴스가 이미 존재합니다. 중복 생성 방지.");
            Destroy(gameObject);
        }
    }
}

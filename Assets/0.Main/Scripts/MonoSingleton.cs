using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool applicationIsQuitting = false;

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
            }
            return _instance;
        }
    }


    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}

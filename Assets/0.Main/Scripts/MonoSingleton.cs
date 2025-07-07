using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    Debug.LogError($"[MonoSingleton] {typeof(T).Name} 인스턴스가 존재하지 않습니다. 씬에 추가해주세요.");
                }
            }
            return _instance;
        }
    }

    
}

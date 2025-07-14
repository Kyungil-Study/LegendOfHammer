using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool applicationIsQuitting = false;
    public virtual void OnInitialize() {}
    

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning($"[SingletonBase] {typeof(T).Name} instance is already quitting. No further access allowed.");
                return null;
            }
            
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
                (_instance as SingletonBase<T>).OnInitialize();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"[SingletonBase] An instance of {typeof(T).Name} already exists. Destroying the new instance.");
            Destroy(gameObject);
        }
        else
        {
            _instance = this as T;
            OnInitialize();
            DontDestroyOnLoad(_instance.gameObject);
        }
    }
    
    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}

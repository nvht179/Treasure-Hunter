using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersistentManager<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        //DontDestroyOnLoad(gameObject);
        Debug.Log($"PersistentManager<{typeof(T).Name}> Awake called");
    }
}
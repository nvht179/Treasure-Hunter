using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersistentManager<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        Debug.Log($"PersistentManager<{typeof(T).Name}> Awake called");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Debug.Log($"Setting Instance of PersistentManager<{typeof(T).Name}> to this");

        Instance = this as T;
        //DontDestroyOnLoad(gameObject);
    }
}
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;


public class ServiceLocator : SerializedMonoBehaviour
{
    public static ServiceLocator instance;
    public static ServiceLocator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<ServiceLocator>();
                if (instance == null)
                {
                    GameObject go = new GameObject("ServiceLocator");
                    instance = go.AddComponent<ServiceLocator>();
                }
            }
            return instance;
        }
    }

    [SerializeField] private Dictionary<Type, object> registeredServices = new();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void RegisterService<T>(T service)
    {
        Type type = typeof(T);
        if(!registeredServices.ContainsKey(type))
        {
            Debug.Log($"Registering service of type {type}");
            registeredServices.Add(type, service);
        }
        else
        {
            Debug.LogWarning($"Service of type {type} is already registered.");
        }
    }

    public void DeregisterService<T>(T Service)
    {
        Type type = typeof(T);

        if (registeredServices.ContainsKey(type))
        {
            Debug.Log($"Deregistering service of type {type}");
            registeredServices.Remove(type);
        }
        else
        {
            Debug.LogWarning($"Service of type {Service} is not registered.");
        }
    }

    public T GetService<T>(object obj)
    {
        Type type = typeof(T);

        if (registeredServices.ContainsKey(type))
        {
            return (T)registeredServices[type];
        }
        else
        {
            Debug.LogError($"Service of type {type} requested from {obj} is not registered.");
            return default;
        }
    }
    private void OnDestroy()
    {
        ResetStatics();
    }
    static void ResetStatics() => instance = null;
}

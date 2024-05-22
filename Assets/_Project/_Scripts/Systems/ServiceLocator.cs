using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;


public class ServiceLocator : SerializedMonoBehaviour
{
    public static ServiceLocator Instance;
    [SerializeField] ServiceLocator locator;
    /*public static ServiceLocator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ServiceLocator>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ServiceLocator");
                    _instance = go.AddComponent<ServiceLocator>();
                }
            }
            return _instance;
        }
    }*/

    [SerializeField] private Dictionary<Type, object> registeredServices = new();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        locator = Instance;
    }

    public void RegisterService<T>(T service)
    {
        Type type = typeof(T);
        if(!registeredServices.ContainsKey(type))
        {
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
            registeredServices.Remove(type);
        }
        else
        {
            Debug.LogWarning($"Service of type {type} is not registered.");
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
}

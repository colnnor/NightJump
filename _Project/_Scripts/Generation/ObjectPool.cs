using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NewObjectPool", menuName = "ScriptableObjects/Pooling/ObjectPool")]
public class ObjectPool : ScriptableObject
{
    [SerializeField] private List<GameObject> pooledGameObjects;
    
    private List<GameObject> pool = new();
    private Transform parent;

    public void Initialize(int count, Transform parent)
    {
        pool.Clear();
        this.parent = parent;

        for (int i = 0; i < count; i++)
        {
            GameObject obj = InstantiatePoolObject(parent);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        if(pool.Count == 0)
        {
            return InstantiatePoolObject(position, rotation);
        }

        for(int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                GameObject obj = pool[i];
                obj.transform.position = position;
                obj.SetActive(true);
                pool.RemoveAt(i);
                return obj;
            }
        }

        throw new Exception("Pool is empty, and the object cannot be created!");
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Add(obj);
    }
    GameObject InstantiatePoolObject(Transform parent)
    {
        int which = Random.Range(0, pooledGameObjects.Count);
        return GameObject.Instantiate(pooledGameObjects[which], parent);
    }

    GameObject InstantiatePoolObject(Vector3 position, Quaternion rotation)
    {
        int which = Random.Range(0, pooledGameObjects.Count);
        return GameObject.Instantiate(pooledGameObjects[which], position, rotation, parent);
    }
}
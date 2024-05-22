using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner
{
    Transform prefab;

    Transform spawnedObject;

    public Spawner(Transform prefab)
    {
        this.prefab = prefab;
    }



    public GameObject Spawn(Vector3 position)
    {
        spawnedObject = GetObject();
        spawnedObject.position = position;
        return spawnedObject.gameObject;
    }

    public void Despawn() => spawnedObject?.gameObject.SetActive(false);
    public Transform GetObject()
    {
        if(spawnedObject == null)
        {
            return Object.Instantiate(prefab);
        }
        return spawnedObject;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private GridData gridData;
    [SerializeField] private List<ObjectPool> objectPools;

    private void Awake()
    {
        foreach (ObjectPool pool in objectPools)
        {
            pool.Initialize(gridData.Size.Squared(), transform);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GridTester : MonoBehaviour
{
    [Title("Grid Data")]
    [SerializeField] private GridData currentGridData;
    [Title("Temp Grid Objects")]
    [SerializeField] private List<GameObject> tempGridObjects;

    [SerializeField] bool destroyOnAwake;
    public void OnEnable()
    {
        if (Application.isPlaying && !destroyOnAwake)
            return;
        DestroyTempGrid();
    }

    [ButtonGroup]
    public void InstantiateTempGrid()
    {
        DestroyTempGrid();

        for (int i = 0; i < currentGridData.Size; i++)
        {
            for (int j = 0; j < currentGridData.Size; j++)
            {
                Vector3 position = new(i * currentGridData.CellSize, 0, j * currentGridData.CellSize);

                GameObject go = Instantiate(tempGridObjects[Random.Range(0, tempGridObjects.Count)], position, Quaternion.identity, transform);

                go.transform.position = position;
            }
        }
    }
    [ButtonGroup]
    public void DestroyTempGrid()
    {
        transform.DestroyChildren();
    }


}
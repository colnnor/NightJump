using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGridData", menuName = "ScriptableObjects/currentGridData")]
[InlineEditor]
[ManageableData]
public class GridData : ScriptableObject
{
    public int Size;

    public float CellSize = 1;
    //public Vector3Int OriginPosition = Vector3Int.zero;
    public float ClampValue = .6f;

    //public int MaxBoulders => Size - (Size / 3);
}

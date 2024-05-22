using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridDataGroup", menuName = "ScriptableObjects/GridDataGroup")]
[InlineEditor]
public class GridDataGroup : SerializedScriptableObject
{
    public Dictionary<int, GridData> GridDatas = new();
}

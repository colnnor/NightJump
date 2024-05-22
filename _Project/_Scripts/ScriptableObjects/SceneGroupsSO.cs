using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneGroupsSO", menuName = "ScriptableObjects/Scene Groups")]
[InlineEditor]
public class SceneGroupsSO : ScriptableObject
{
    public SceneGroup[] sceneGroups;
}

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CustomUIComponent : MonoBehaviour
{
    private void Awake() => Init();
    private void OnValidate() => Init();

    [Button("Update")]
    private void Init()
    {
        Setup();
        Configure();
    }
    
    public abstract void Setup();
    public abstract void Configure();
}

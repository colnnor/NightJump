using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCamera : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private GameObject cube;

    private void OnEnable() => inputReader.LightEnabledEvent += OnLightEnabled;
    private void OnLightEnabled(bool isEnabled) => cube.SetActive(!isEnabled);
}

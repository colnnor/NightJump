using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToWorldPosition : MonoBehaviour
{
    [SerializeField] private RectTransform element;
    [SerializeField] private Camera cam;
    [SerializeField] float distance;

    private void Update()
    {
        SetPosition();
    }
    [Button("Set Position")]
    private void SetPosition()
    {
        Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, element.position);
        Vector3 worldPosition = cam.ScreenToWorldPoint(screenPoint);

        transform.position = worldPosition + cam.transform.forward * distance;
    }
}

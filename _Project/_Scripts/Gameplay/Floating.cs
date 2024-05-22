using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    [SerializeField] private float amplitude = 1;
    [SerializeField] private float frequency = 1f;

    float time;
    private void Update()
    {
        time += Time.deltaTime;
        float yPosition = amplitude * Mathf.Sin(frequency * time);

        transform.localPosition = new(transform.localPosition.x, 1 + yPosition, transform.localPosition.z);

    }
}

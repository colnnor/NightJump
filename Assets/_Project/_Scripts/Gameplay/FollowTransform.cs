using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private bool followPosition = true;
    [Title("Settings")]
    [SerializeField] private bool offsetPosition = false;
    [ShowIf("offsetPosition", Value = true)]
    [SerializeField] private Vector3 positionOffset;
    /*
    [Space()]
    [SerializeField] private bool enforceConstraints;
    [ShowIf("enforceConstraints", Value = true)]
    [BoxGroup("Constraints/Constraints")]
    [HorizontalGroup("Constraints")]
    [SerializeField] private bool x;
    [ShowIf("enforceConstraints", Value = true)]
    [BoxGroup("Constraints/Constraints")]
    [HorizontalGroup("Constraints")]
    [SerializeField] private bool y;
    [ShowIf("enforceConstraints", Value = true)]
    [BoxGroup("Constraints/Constraints")]
    [HorizontalGroup("Constraints")]
    [SerializeField] private bool z;*/

    private void Update()
    {
        if(!target || !followPosition) return;

        transform.position = offsetPosition ? target.position + positionOffset : target.position;

    }
}

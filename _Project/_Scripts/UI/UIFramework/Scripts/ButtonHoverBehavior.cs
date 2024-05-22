using DG.Tweening;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonHoverBehavior", menuName = "CustomUI/ButtonHoverBehavior", order = 1)]
public class ButtonHoverBehavior : ScriptableObject
{
    public float verticalMoveAmount = 30f;
    [SerializeField] private float moveTime = .1f;
    [SerializeField] private Ease easeType = Ease.OutQuad;
    [SerializeField] private float scaleTime;

    public Action<Transform, Vector3> SelectAction;
    public Action<Transform, Vector3> DeselectAction;

    public void Init()
    {
        SelectUI();
        DeselectUI();
    }
    public void SelectUI()
    {
        SelectAction = (transform, startPosition) =>
        {
            transform.DOMoveY(startPosition.y + verticalMoveAmount, moveTime).SetEase(easeType);/*
            transform.DOScale(transform.scale * 1.1f, scaleTime).SetEase(easeType);*/
        };
    }

    public void DeselectUI()
    {
        DeselectAction = (transform, startPosition) =>
        {
            transform.DOMoveY(startPosition.y, moveTime).SetEase(easeType);
            transform.DOScale(startPosition, scaleTime).SetEase(easeType);
        };
    }
}

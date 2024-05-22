using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public PageAnimation EnterAnimation;
    public PageAnimation ExitAnimation;

    [SerializeField] private ContainerAnimationData animationData;

    public void AnimateContainerIn()
    {
        Transform menuTransform = transform;
        float duration = animationData.Duration;
        Ease ease = animationData.Ease;

        switch (EnterAnimation)
        {
            case PageAnimation.SlideIn:
                Debug.Log("SLiding in");

                menuTransform.DOLocalMove(Vector3.zero, duration).SetEase(ease);
                break;
            case PageAnimation.ScaleIn:
                menuTransform.localPosition = Vector3.zero;
                menuTransform.DOScale(Vector3.one, duration).SetEase(ease);
                break;
        }
    }
    public void AnimateContainerOut()
    {
        Transform menuTransform = transform;
        float duration = animationData.Duration;
        Ease ease = animationData.Ease;

        switch (ExitAnimation)
        {
            case PageAnimation.SlideOutLeft:
                menuTransform.DOLocalMoveX(-Screen.width, duration).SetEase(ease);
                break;
            case PageAnimation.SlideOutRight:
                menuTransform.DOLocalMoveX(Screen.width, duration).SetEase(ease);
                break;
            case PageAnimation.SlideOutTop:
                menuTransform.DOLocalMoveY(Screen.height, duration).SetEase(ease);
                break;
            case PageAnimation.SlideOutBottom:
                menuTransform.DOLocalMoveY(-Screen.height, duration).SetEase(ease);
                break;
            case PageAnimation.ScaleOut:
                menuTransform.DOScale(Vector3.zero, duration).SetEase(ease);
                break;
        }
    }

    public void ImmediateOut()
    {
        transform.localPosition = new Vector3(-Screen.width, 0, 0);
    }
}

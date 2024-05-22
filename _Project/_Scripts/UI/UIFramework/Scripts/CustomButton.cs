using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : CustomUIComponent, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Serializable]
    public class ButtonClickEvent : UnityEvent { }

    //[SerializeField] private UIButtonManager buttonManager;

    [Title("Theme")]
    [ColorPalette("Menu Theme")]
    [HideLabel]
    [SerializeField] private Color primaryColorBG = new(0.95f, 0.95f, 0.95f, 1f);
    
    [BoxGroup("Visual", ShowLabel = false)]
    [SerializeField] private Image buttonVisual;

    [BoxGroup("Visual", ShowLabel = false)]
    [ShowIf("buttonVisual")]
    [SerializeField] private Color imageColor;
    [SerializeField] private ButtonAnimationValues buttonAnimationValues;
    [SerializeField] private bool animateSelf;
    [HideIf("animateSelf")]
    [SerializeField] private Transform transformToAnimate;

    [Title("Events")]
    public ButtonClickEvent onClick = new();
    [SerializeField] private EventChannel onClickEvent;

    private Transform animationTransform;
    private Vector3 startScale;

    public override void Configure()
    {
        animationTransform = animateSelf ? transform : transformToAnimate;
    }
    public override void Setup()
    {
        if(GetComponent<Image>() != null)
        {
            buttonVisual = GetComponent<Image>();
        }
        if (!buttonVisual) return;
        
        buttonVisual.color = imageColor;
    }
    private void Start()
    {
        if(animateSelf || transformToAnimate == null) animationTransform = transform;
        startScale = animateSelf ? transform.localScale : animationTransform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerClickAnimation();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        PointerReleaseAnimation();

        //if (buttonManager.CurrentSelected != gameObject) return;
        onClickEvent?.Invoke(new Empty());
        onClick?.Invoke();
    }
    #region pointer hover methods
    public void OnSelect(BaseEventData eventData) => PointerEnterAnimation();/*
        if (!buttonManager) return;

        buttonManager.CurrentSelected = gameObject;
        buttonManager.LastSelecterd = gameObject;
        buttonManager.LastSelectedIndex = buttonManager.Buttons.IndexOf(gameObject);*/
    public void OnDeselect(BaseEventData eventData) => PointerExitAnimation();/*
        if (!buttonManager) return;
        buttonManager.CurrentSelected = null;*/
    public void OnPointerEnter(PointerEventData eventData) => OnSelect(eventData);
    public void OnPointerExit(PointerEventData eventData) => OnDeselect(eventData);

    private void OnDisable()
    {
        KillAll();
    }
    public void PointerClickAnimation()
    {
        animationTransform?.DOScale(startScale * .95f, buttonAnimationValues.scaleTime).SetEase(buttonAnimationValues.easeType);
    }
    public void PointerReleaseAnimation()
    {
        animationTransform?.DOScale(startScale * 1.1f, buttonAnimationValues.scaleTime).SetEase(buttonAnimationValues.easeType);
    }

    public void PointerEnterAnimation()
    {
        animationTransform?.DOLocalMoveY(buttonAnimationValues.verticalMoveAmount, buttonAnimationValues.moveTime).SetEase(buttonAnimationValues.easeType);
        animationTransform?.DOScale(startScale * 1.1f, buttonAnimationValues.scaleTime).SetEase(buttonAnimationValues.easeType);
    }

    public void PointerExitAnimation()
    {
        animationTransform?.DOLocalMoveY(-buttonAnimationValues.verticalMoveAmount, buttonAnimationValues.moveTime).SetEase(buttonAnimationValues.easeType);
        animationTransform?.DOScale(startScale, buttonAnimationValues.scaleTime).SetEase(buttonAnimationValues.easeType);
    }

    public void KillAll()
    {
        animationTransform?.DOKill();
    
    }

    #endregion
}

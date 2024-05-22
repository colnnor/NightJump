using DG.Tweening;
using UnityEngine;

public class ButtonAnimationChild : MonoBehaviour
{

    [SerializeField] private ButtonAnimationValues buttonAnimationValues;

    private Vector3 startPosition;
    private Vector3 startScale;

    private void Start()
    {
        startPosition = transform.localPosition;
        startScale = transform.localScale;
    }
    public void PointerClickAnimation()
    {
        Vector3 scale = transform.localScale * .95f;
        transform.DOScale(scale, buttonAnimationValues.scaleTime).SetEase(buttonAnimationValues.easeType);
    }
    public void PointerReleaseAnimation()
    {
        Vector3 scale = transform.localScale / .95f;
        transform.DOScale(scale, buttonAnimationValues.scaleTime).SetEase(buttonAnimationValues.easeType);
    }

    public void PointerEnterAnimation()
    {
        Debug.Log(this.gameObject.name + gameObject.transform.parent.name);
        transform.DOLocalMoveY(buttonAnimationValues.verticalMoveAmount, buttonAnimationValues.moveTime).SetEase(buttonAnimationValues.easeType);
        transform.DOScale(startScale * 1.1f, buttonAnimationValues.scaleTime).SetEase(buttonAnimationValues.easeType);
    }

    public void PointerExitAnimation()
    {
        transform.DOLocalMoveY(-buttonAnimationValues.verticalMoveAmount, buttonAnimationValues.moveTime).SetEase(buttonAnimationValues.easeType);
        transform.DOScale(startScale, buttonAnimationValues.scaleTime).SetEase(buttonAnimationValues.easeType);
    }
}
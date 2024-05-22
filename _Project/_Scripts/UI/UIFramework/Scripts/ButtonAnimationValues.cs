using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonAnimationValues", menuName = "CustomUI/Button Animation Values")]
[InlineEditor]
public class ButtonAnimationValues : ScriptableObject
{
    [Title("Button Animation Values")]
    public float verticalMoveAmount = 10f;
    public float moveTime = .25f;
    public float scaleTime = .25f;
    public Ease easeType = Ease.InOutQuad;

}

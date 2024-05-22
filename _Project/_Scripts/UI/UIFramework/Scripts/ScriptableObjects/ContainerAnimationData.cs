using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ContainerAnimationData", menuName = "CustomUI/ContainerAnimationData")]
[InlineEditor]
public class ContainerAnimationData : ScriptableObject
{
    public float Duration = .2F;
    public Ease Ease = Ease.InOutExpo;
}

using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

[CreateAssetMenu(fileName = "LevelMovementValues", menuName = "ScriptableObjects/LevelMovementValues")]
public class LevelMovementValues : ScriptableObject
{
    [Title("Level Movement Values")]
    public float ArcHeight = 5f;
    public float Speed = 4f;
    public Ease EaseType = Ease.InOutSine;
}

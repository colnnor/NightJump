using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMovementValues", menuName = "ScriptableObjects/EnemyMovementValues", order = 1)]
[InlineEditor]
public class EnemyMovementValues : ScriptableObject
{
    public AudioClip JumpSound;
    public float TranslationTime = .4f;
    public float MoveCooldownTime = .1f;
}

using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    Animator animator;

    static readonly int JumpHash = Animator.StringToHash("Jump");

    readonly Dictionary<int, float> animationDuration = new()
    {
        {JumpHash, .1f }
    };

    const float k_crossfadeDuration = .1f;


    private void Awake() => animator = GetComponent<Animator>();

    public float Jump() => PlayAnimation(JumpHash);

    float PlayAnimation(int animationHash)
    {
        animator.CrossFade(animationHash, k_crossfadeDuration);
        return animationDuration[animationHash];
    }
}
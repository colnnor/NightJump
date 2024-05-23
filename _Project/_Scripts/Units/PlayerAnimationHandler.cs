using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] private float animationTime = .15f;
    [SerializeField] private InputReader inputReader;

    private const string JUMP = "Jump";
    [SerializeField] Animator animator;
    private bool isAnimating;


    private void Awake()
    {
        inputReader.AnyPressed += AnyPressed;


    }

    private void OnDestroy()
    {
        inputReader.AnyPressed -= AnyPressed;
    }
    private void AnyPressed()
    {
        if (isAnimating) return;
        isAnimating = true;

        Debug.Log( this);
        animator.SetTrigger(JUMP);
        StartCoroutine(ResetIsAnimating());
    }

    IEnumerator ResetIsAnimating()
    {
        yield return Helpers.GetWait(animationTime);

        isAnimating = false;
    }
}

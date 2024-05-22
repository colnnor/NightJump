using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] private PlayerController playerMovement;

    [SerializeField] private float animationTime = .15f;
    [SerializeField] private InputReader inputReader;

    private const string JUMP = "Jump";
    public Animator animator;
    private bool isAnimating;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputReader.AnyPressed += AnyPressed;
    }
    private void Start()
    {
        playerMovement = ServiceLocator.Instance.GetService<PlayerController>(this);
    }
    private void AnyPressed()
    {
        if (isAnimating) return;
        isAnimating = true;
        animator.SetTrigger(JUMP);
        StartCoroutine(ResetIsAnimating());
    }

    IEnumerator ResetIsAnimating()
    {
        yield return Helpers.GetWait(animationTime);

        isAnimating = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;



public class FeedbacksManager : MonoBehaviour, IDependencyProvider
{
    [Provide] FeedbacksManager ProvideFeedbacksManager() => this;

    [SerializeField] private MMFeedbacks jumpFeedback;
    [SerializeField] private MMFeedbacks damageFeedback;
    [SerializeField] private MMFeedbacks uiFeedback;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<FeedbacksManager>(this);
    }
    private void OnEnable()
    {
        PlayerController.OnPlayerDamage += PlayDamageFeedback;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerDamage -= PlayDamageFeedback;
    }
    public void PlayJumpFeedback() => jumpFeedback.PlayFeedbacks();
    public void PlayDamageFeedback(int value)
    {
        damageFeedback.PlayFeedbacks();
        uiFeedback.PlayFeedbacks();
    }

}

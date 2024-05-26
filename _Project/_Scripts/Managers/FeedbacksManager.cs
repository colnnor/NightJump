using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;



public class FeedbacksManager : MonoBehaviour
{

    [SerializeField] private MMFeedbacks jumpFeedback;
    [SerializeField] private MMFeedbacks damageFeedback;
    [SerializeField] private MMFeedbacks uiFeedback;
    [SerializeField] private MMFeedbacks gemCollectedFeedback;
    [SerializeField] private MMFeedbacks platformLanded;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<FeedbacksManager>(this);
    }
    private void OnEnable()
    {
        Gem.GemCollected += PlayGemCollectedFeedback;
        PlatformMovement.OnPlatformMovementComplete += PlayPlatformLandedFeedback;
        PlayerController.OnPlayerDamage += PlayDamageFeedback;
    }

    private void PlayPlatformLandedFeedback() => platformLanded.PlayFeedbacks();


    private void OnDisable()
    {
        Gem.GemCollected -= PlayGemCollectedFeedback;
        PlatformMovement.OnPlatformMovementComplete -= PlayPlatformLandedFeedback;
        ServiceLocator.Instance.DeregisterService<FeedbacksManager>(this);
        PlayerController.OnPlayerDamage -= PlayDamageFeedback;
    }
    public void PlayGemCollectedFeedback(bool value) => gemCollectedFeedback.PlayFeedbacks();
    public void PlayJumpFeedback() => jumpFeedback.PlayFeedbacks();
    public void PlayDamageFeedback(int value)
    {
        damageFeedback.PlayFeedbacks();
        uiFeedback.PlayFeedbacks();
    }

}

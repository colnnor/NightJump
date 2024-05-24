using DG.Tweening;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static event Action Play;

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gameOverMenu;

    [SerializeField] private float animTime;
    [SerializeField] private Ease easeType = Ease.InOutQuart;

    [SerializeField] private EventChannel sceneLoaded;

    [Title("Feedbacks")]
    [SerializeField] private MMF_Player feedbacks;

    private Dictionary<MenuState, GameObject> menuScales;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<MainMenuManager>(this);
        menuScales = new()
        {
            { MenuState.Main, mainPanel },
            { MenuState.Settings, settingsPanel },
            { MenuState.GameOver, gameOverMenu },
        };
    }


    private void OnDestroy()
    {
        ServiceLocator.Instance.DeregisterService<MainMenuManager>(this);
    }
    private void Start()
    {
        SetMenuState(MenuState.Main);
        sceneLoaded?.Invoke(new Empty());
    }

    public void UIClick()
    {
        AudioManager.Instance.PlayOneShot(SFXType.ButtonClickPositive, randomPitch: true);
        feedbacks?.PlayFeedbacks();
    }
    public void PlayPressed()
    {
        Play?.Invoke();
    }


    public void ShowMainPanel() => SetMenuState(MenuState.Main, animTime);
    public void ShowSettings() => SetMenuState(MenuState.Settings, animTime);
    public void ShowConfirmQuitPanel() => SetMenuState(MenuState.GameOver, animTime);

    private void SetMenuState(MenuState state, float? animTime = 0)
    {
        float animationTime = animTime ?? this.animTime;

        foreach (var menu in menuScales)
        {
            if (!menu.Value.activeSelf)
                return;

            if (menu.Key == state)
            {
                menu.Value.transform.DOScale(Vector3.one, animationTime).SetEase(easeType);
            }
            else
            {
                menu.Value.transform.DOScale(Vector3.zero, animationTime).SetEase(easeType);
            }
        }
    }
}

public enum ContainerAnimation
{
    Enter,
    Exit
}
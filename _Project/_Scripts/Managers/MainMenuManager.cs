using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour, IDependencyProvider
{
    [Provide] private MainMenuManager ProvideMainMenuManager() => this;

    [SerializeField] private Container mainPanel;
    [SerializeField] private Container settingsPanel;
    [SerializeField] private Container confirmQuitPanel;
    [SerializeField] private EventChannel sceneLoaded;

    [Title("Feedbacks")]
    [SerializeField] private MMF_Player feedbacks;

    private Vector3 center = new(0, 0, 0);
    private Vector3 left = new(-1920, 0, 0);
    private Vector3 right = new(1920, 0, 0);
    private Vector3 top = new(0, 1080, 0);

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService<MainMenuManager>(this);
    }
    private void Start()
    {
        sceneLoaded?.Invoke(new Empty());
    }
    public void UIClick()
    {
        AudioManager.Instance.PlayOneShot(SFXType.ButtonClickPositive, randomPitch: true);
        feedbacks?.PlayFeedbacks();
    }
    [Button]
    void ManualMainPage()
    {
        mainPanel.transform.localPosition = center;
        settingsPanel.transform.localPosition = Vector3.zero.With(x: 1920);
        confirmQuitPanel.transform.localPosition = Vector3.zero.With(y: -1080);
    }
    [Button]
    void ManualSettingsPage()
    {
        mainPanel.transform.localPosition = Vector3.zero.With(x: -1920);
        settingsPanel.transform.localPosition = center;
        confirmQuitPanel.transform.localPosition = Vector3.zero.With(y: -1080);
    }
    [Button]
    void ManualConfirmQuitPage()
    {
        mainPanel.transform.localPosition = Vector3.zero.With(x: -1920);
        settingsPanel.transform.localPosition = Vector3.zero.With(x: 1920);
        confirmQuitPanel.transform.localPosition = center;
    }
    public void ShowMainPanel()
    {
        settingsPanel.AnimateContainerOut();
        confirmQuitPanel.AnimateContainerOut();
        mainPanel.AnimateContainerIn();
    }

    public void ShowSettings()
    {
        settingsPanel.AnimateContainerIn();
        mainPanel.AnimateContainerOut();
        confirmQuitPanel.AnimateContainerOut();
    }

    public void ShowConfirmQuitPanel()
    {
        confirmQuitPanel.AnimateContainerIn();
        settingsPanel.AnimateContainerOut();
        mainPanel.AnimateContainerOut();
    }
}

public enum ContainerAnimation
{
    Enter,
    Exit
}
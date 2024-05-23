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
    [SerializeField] private GameObject confirmQuitPanel;
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

    public void SetMenuState(MenuState state)
    {
        settingsPanel.SetActive(state == MenuState.Settings);
        confirmQuitPanel.SetActive(state == MenuState.GameOver);
        mainPanel.SetActive(state == MenuState.Main);
    }

    public void ShowMainPanel() => SetMenuState(MenuState.Main);
    public void ShowSettings() => SetMenuState(MenuState.Settings);
    public void ShowConfirmQuitPanel() => SetMenuState(MenuState.GameOver);


    #region manuals
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
    #endregion
}

public enum ContainerAnimation
{
    Enter,
    Exit
}
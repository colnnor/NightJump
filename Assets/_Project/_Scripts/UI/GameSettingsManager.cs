using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSettingsManager : MonoBehaviour
{
    private void Start()
    {
    }

    public void PlayUISFX() => AudioManager.Instance.PlayOneShot(SFXType.ButtonClickPositive);
    public void UpdateSFXVolume(float volume) => AudioManager.Instance.UpdateSFXVolume(volume);
    public void UpdateMusicVolume(float value) => AudioManager.Instance.UpdateMusicVolume(value);
    public void UpdateMasterVolume(float value) => AudioManager.Instance.UpdateMasterVolume(value);

    public void ViewToWindowed() => Screen.fullScreenMode = FullScreenMode.Windowed;
    public void ViewToFullscreen() => Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

    public void UpdateQuality(int qualityLevel) => QualitySettings.SetQualityLevel(qualityLevel);

}

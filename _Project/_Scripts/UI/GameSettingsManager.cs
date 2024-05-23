using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;

public class GameSettingsManager : MonoBehaviour
{
    [SerializeField] private AudioSettings audioSettings;
    [Space(10)]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        UpdateSliders(audioSettings);
    }
    public void UpdateSliders(AudioSettings settings)
    {
        masterSlider.value = settings.MasterVolume;
        musicSlider.value = settings.MusicVolume;
        sfxSlider.value = settings.SFXVolume;
    }
    public void PlayUISFX() => AudioManager.Instance.PlayOneShot(SFXType.ButtonClickPositive);
    public void UpdateSFXVolume(float volume) => AudioManager.Instance.UpdateSFXVolume(volume);
    public void UpdateMusicVolume(float value) => AudioManager.Instance.UpdateMusicVolume(value);
    public void UpdateMasterVolume(float value) => AudioManager.Instance.UpdateMasterVolume(value);

    public void ViewToWindowed() => Screen.fullScreenMode = FullScreenMode.Windowed;
    public void ViewToFullscreen() => Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

    public void UpdateQuality(int qualityLevel) => QualitySettings.SetQualityLevel(qualityLevel);

}

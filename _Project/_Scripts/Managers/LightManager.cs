using Sirenix.OdinInspector;
using System.Collections;
using System;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;

    public static event Action DelayedLightOff;
    public static event Action<bool> LightEnabled;


    [Title("References")]
    [SerializeField] private InputReader inputReader;
    [Title("Light Objects")]
    [SerializeField] private GameObject sun;
    [SerializeField] private GameObject spot;
    [Title("Settings")]
    [SerializeField] private float lightsOffDelay = 2f;

    bool moving;


    private void Start()
    {
        cameraManager = ServiceLocator.Instance.GetService<CameraManager>(this);
        LightsOffDelayed();
    }

    private void OnEnable()
    {
        inputReader.LightEnabledEvent += ToggleLight;
        GridManager.GemCollected += EnableLight;
        PlatformMovement.OnPlatformMovementComplete += LightsOffDelayed;
        PlayerController.OnPlayerDamage += DisableLight;
        GameManager.OnGameEnd += GameOver;
    }
    private void OnDisable()
    {
        GridManager.GemCollected -= EnableLight;
        inputReader.LightEnabledEvent -= ToggleLight;
        PlayerController.OnPlayerDamage -= DisableLight;
        PlatformMovement.OnPlatformMovementComplete -= LightsOffDelayed;
        GameManager.OnGameEnd -= GameOver;
    }

    void DisableLight(int a)
    {
        DisableLight();
    }
    public void LightsOffDelayed() => StartCoroutine(LightsOffDelay());

    public void GameOver()
    {
        StopAllCoroutines();
        moving = false;
        ToggleLight(false);
    }
    public IEnumerator LightsOffDelay()
    {
        yield return Helpers.GetWait(lightsOffDelay);
        moving = false;
        ToggleLight(false);
        DelayedLightOff?.Invoke();
    }

    [ButtonGroup]
    void EnableLight(bool value = true)
    {
        Debug.Log("Enabling light");
        ToggleLight(true);
        moving = true;
    }

    [ButtonGroup]
    void DisableLight() => ToggleLight(false);
    public void ToggleLight(bool lightActive)
    {
        if (moving) return;

        LightEnabled?.Invoke(lightActive);
        cameraManager?.ChangeCamera(lightActive);
        spot.SetActive(!lightActive);
        sun.SetActive(lightActive);
    }
}

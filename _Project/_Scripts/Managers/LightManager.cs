using Sirenix.OdinInspector;
using System.Collections;
using System;
using UnityEngine;
using DG.Tweening;
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
    private float remainingDelay;

    private void Start()
    {
        cameraManager = ServiceLocator.Instance.GetService<CameraManager>(this);
        remainingDelay = lightsOffDelay;
        LightsOffDelayed();
    }

    private void OnEnable()
    {
        inputReader.LightEnabledEvent += ToggleLight;
        GridManager.GemCollected += CollectedGem;
        PlatformMovement.OnPlatformMovementComplete += LightsOffDelayed;
        PlayerController.OnPlayerDamage += DisableLight;
        GameManager.OnGameEnd += GameOver;
        GameManager.OnGamePause += Pause;
        GameManager.OnGameResume += Resume;
    }

    private void Resume()
    {
        if(!moving) return;
        EnableLight();
    }

    void CollectedGem(bool b)
    {
        EnableLight();
        moving = true;
    }
    private void Pause()
    {
        EnableLight(false);
    }

    private void OnDisable()
    {
        GridManager.GemCollected -= CollectedGem;
        inputReader.LightEnabledEvent -= ToggleLight;
        PlayerController.OnPlayerDamage -= DisableLight;
        PlatformMovement.OnPlatformMovementComplete -= LightsOffDelayed;
        GameManager.OnGameEnd -= GameOver;
        GameManager.OnGamePause -= Pause;
    }

    void DisableLight(int a)
    {
        EnableLight(false);
    }

    public void LightsOffDelayed()
    {
        StartCoroutine(LightsOffDelay(remainingDelay));
    }

    public IEnumerator LightsOffDelay(float delay)
    {
        float timer = 0;
        while (timer < delay)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        moving = false;
        ToggleLight(false);
        DelayedLightOff?.Invoke();
    }

    [ButtonGroup]
    void EnableLight(bool value = true)
    {
        ToggleLight(value);
    }

    public void GameOver()
    {
        StopAllCoroutines();
        moving = false;
        ToggleLight(false);
    }

    public void ToggleLight(bool lightActive)
    {
        LightEnabled?.Invoke(lightActive);
        cameraManager?.ChangeCamera(lightActive);
        spot.SetActive(!lightActive);
        sun.SetActive(lightActive);
    }
}

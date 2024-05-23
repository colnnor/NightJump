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
    private Coroutine lightsOffCoroutine;
    private float remainingDelay;
    private float startedTime;
    private float timer;

    private void Start()
    {
        cameraManager = ServiceLocator.Instance.GetService<CameraManager>(this);
        remainingDelay = lightsOffDelay;
        LightsOffDelayed();
    }

    private void OnEnable()
    {
        inputReader.LightEnabledEvent += ToggleLight;
        GridManager.GemCollected += EnableLight;
        PlatformMovement.OnPlatformMovementComplete += LightsOffDelayed;
        PlayerController.OnPlayerDamage += DisableLight;
        GameManager.OnGamePause += Pause;
        GameManager.OnGameResume += Resume;
        GameManager.OnGameEnd += GameOver;
    }
    private void OnDisable()
    {
        GridManager.GemCollected -= EnableLight;
        inputReader.LightEnabledEvent -= ToggleLight;
        PlayerController.OnPlayerDamage -= DisableLight;
        PlatformMovement.OnPlatformMovementComplete -= LightsOffDelayed;
        GameManager.OnGamePause -= Pause;
        GameManager.OnGameResume -= Resume;
        GameManager.OnGameEnd -= GameOver;
    }

    void DisableLight(int a)
    {
        EnableLight(false);
    }

    void Pause()
    {
        if (lightsOffCoroutine != null)
        {
            StopCoroutine(lightsOffCoroutine);
            remainingDelay = lightsOffDelay - timer;
        }
    }
    void Resume()
    {
        LightsOffDelayed();
    }
    public void LightsOffDelayed()
    {
        if (lightsOffCoroutine != null)
        {
            StopCoroutine(lightsOffCoroutine);
        }
        lightsOffCoroutine = StartCoroutine(LightsOffDelay(remainingDelay));
    }

    public IEnumerator LightsOffDelay(float delay)
    {
        while (timer < delay)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        moving = false;
        ToggleLight(false);
        DelayedLightOff?.Invoke();
        timer = 0;
    }

    [ButtonGroup]
    void EnableLight(bool value = true)
    {
        ToggleLight(true);
        moving = true;
    }

    public void GameOver()
    {
        StopAllCoroutines();
        moving = false;
        ToggleLight(false);
    }

    public void ToggleLight(bool lightActive)
    {
        if (moving)
            return;

        LightEnabled?.Invoke(lightActive);
        cameraManager?.ChangeCamera(lightActive);
        spot.SetActive(!lightActive);
        sun.SetActive(lightActive);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Eflatun.SceneReference;
using NUnit.Framework;

public enum SceneType
{
    MainMenu,
    GamePlay
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private bool disableCanvasOnStart;
    [SerializeField] private GameObject pressEnterToPlay;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private GameObject canvasCamera;
    [SerializeField] private Image progressBar;

    [SerializeField] private List<SceneReference> scenesToLoad;

    int currenScene;

    public static event Action<SceneType> OnSceneLoaded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadScene(0);
    }

    public void LoadNextScene()
    {
        currenScene++;
        LoadScene(currenScene);
    }

    public void LoadScene(int index)
    {
        StartCoroutine(LoadSceneAsync(index));
    }

    public void LoadScene(SceneReference reference)
    {
        StartCoroutine(LoadSceneAsync(scenesToLoad.IndexOf(reference)));
    }

    private IEnumerator LoadSceneAsync(int index)
    {
        // Begin to load the Scene you specify
        SceneReference sceneReference = scenesToLoad[index];
        var scene = SceneManager.LoadSceneAsync(sceneReference.Path);
        scene.allowSceneActivation = false;

        SceneType type = index == 0 ? SceneType.MainMenu : SceneType.GamePlay;

        if(type != SceneType.MainMenu)
        EnableLoadingCanvas();

        // While the Scene is loading, output the current progress
        while (scene.progress < 0.9f)
        {
            if (progressBar)
            {
                progressBar.fillAmount = scene.progress;
            }
            yield return null; // Wait for next frame
        }

        scene.allowSceneActivation = true;

        OnSceneLoaded?.Invoke(type);

        if (type == SceneType.MainMenu)
        {
            EnableLoadingCanvas(false);
        }
    }

    public void EnableLoadingCanvas(bool enabled = true)
    {
        pressEnterToPlay.SetActive(enabled);
        canvasCamera.SetActive(enabled);
        loadingCanvas.SetActive(enabled);
    }
}

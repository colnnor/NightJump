using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public enum SceneType
{
    MainMenu,
    GamePlay
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private bool disableCanvasOnStart;
    [SerializeField] private GameObject pressSpaceToPlay;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private GameObject canvasCamera;
    [SerializeField] private Image progressBar;

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
        LoadScene(1);
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

    private IEnumerator LoadSceneAsync(int index)
    {
        // Begin to load the Scene you specify
        var scene = SceneManager.LoadSceneAsync(index);
        scene.allowSceneActivation = false;

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

        SceneType type = index == 0 ? SceneType.MainMenu : SceneType.GamePlay;
        OnSceneLoaded?.Invoke(type);

        if (disableCanvasOnStart)
        {
            EnableLoadingCanvas(false);
        }
    }

    public void EnableLoadingCanvas(bool enabled = true)
    {
        canvasCamera.SetActive(enabled);
        loadingCanvas.SetActive(enabled);
    }
}

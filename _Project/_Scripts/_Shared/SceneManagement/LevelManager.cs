using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using Sirenix.OdinInspector;

public enum SceneType
{
    MainMenu,
    GamePlay
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private bool disableCanvasOnStart;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private GameObject canvasCamera;
    [SerializeField] private Image progressBar;
    [SerializeField] private List<SceneReference> scenesToLoad;

    public Dictionary<SceneReference, SceneType> ScenesDictionary;

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
        ScenesDictionary = new()
        {
            { scenesToLoad[0] , SceneType.MainMenu},
            { scenesToLoad[1] , SceneType.GamePlay }
        };

        LoadScene(0);
    }

    public void LoadNextScene()
    {
        currenScene++;
        LoadScene(currenScene);
    }
    public async void LoadScene(int index)
    {
        currenScene = index;

        SceneReference reference = scenesToLoad[index];
        var scene = SceneManager.LoadSceneAsync(reference.Path);
        scene.allowSceneActivation = false; 

        EnableLoadingCanvas();
        do
        {
            await Task.Delay(100);
            if(progressBar) progressBar.fillAmount = scene.progress;
        } while (scene.progress < 0.9f);

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public async void LoadScene(int index)
    {
        //SceneReference reference = scenesToLoad[index];
        var scene = SceneManager.LoadSceneAsync(index);//reference.Path);
        scene.allowSceneActivation = false; 

        EnableLoadingCanvas();
        do
        {
            await Task.Delay(100);
            if(progressBar) progressBar.fillAmount = scene.progress;
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;

        SceneType type = index == 1 ? SceneType.MainMenu : SceneType.GamePlay;
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

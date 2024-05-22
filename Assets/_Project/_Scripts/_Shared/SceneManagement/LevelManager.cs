using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private bool disableCanvasOnStart;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private GameObject canvasCamera;
    [SerializeField] private Image progressBar;
    [SerializeField] private List<SceneReference> scenesToLoad;

    int currenScene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
        if(disableCanvasOnStart)
        EnableLoadingCanvas(false); 
    }

    public void EnableLoadingCanvas(bool enabled = true)
    {
        canvasCamera.SetActive(enabled);
        loadingCanvas.SetActive(enabled);
    }
}

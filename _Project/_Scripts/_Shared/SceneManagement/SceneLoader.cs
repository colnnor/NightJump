using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] bool useProgressBar;
    [SerializeField] bool disableOnSceneLoad;
    [SerializeField] Image loadingBar;
    [SerializeField] float fillSpeed = .5f;
    [SerializeField] Canvas loadingCanvas;
    [SerializeField] Camera loadingCamera;
    [SerializeField] SceneGroupsSO sceneGroupsSO;

    private int currentGroupIndex;

    float targetProgress;
    bool isLoading;

    public readonly SceneGroupManager manager = new();

    private void Awake()
    {
        manager.OnSceneLoaded += sceneName => Debug.Log("Scene loaded: " + sceneName);
        manager.OnSceneUnloaded += sceneName => Debug.Log("Scene unloaded: " + sceneName);
        manager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");
    }

    public async void LoadGroup(int index)
    {
        await LoadSceneGroup(index);
    }

    public async void LoadNextGroup()
    {
        await LoadSceneGroup(currentGroupIndex + 1);
    }

    async void Start()
    {
        await LoadSceneGroup(0);
    }

    private void Update()
    {
        if(!isLoading || !useProgressBar) return;

        float currentFillAmount = loadingBar.fillAmount;
        float progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

        float dynmaicFillSpeed = fillSpeed * progressDifference;

        loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, targetProgress, dynmaicFillSpeed * Time.deltaTime);
    }

    private async Task LoadSceneGroup(int index)
    {
        if(loadingBar)
            loadingBar.fillAmount = 0;
        
        targetProgress = 1;


        if (index < 0 || index >= sceneGroupsSO.sceneGroups.Length)
        {
            Debug.LogError("Invalid scene group index: " + index);
            return;
        }

        LoadingProgress progress = new();
        progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);

        EnableLoadingCanvas();
        await manager.LoadScenes(sceneGroupsSO.sceneGroups[index], progress);
        currentGroupIndex = index;
        if(disableOnSceneLoad)
            EnableLoadingCanvas(false);
    }

    public void EnableLoadingCanvas(bool enable = true)
    {
        isLoading = enable;
        loadingCanvas.gameObject.SetActive(enable);
        loadingCamera.gameObject.SetActive(enable);
    }
}

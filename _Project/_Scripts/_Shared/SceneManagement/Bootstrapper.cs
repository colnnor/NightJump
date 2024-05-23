using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : PersistentSingleton<Bootstrapper>
{
    [SerializeField] static bool staticStartWithBootstrapper = true;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static async void Init()
    {
        if (!staticStartWithBootstrapper)
        {
            Debug.Log("Bootstrapper disabled...");
            return;
        }

        Debug.Log("Bootstrapper...");
        await SceneManager.LoadSceneAsync("Bootstrapper", LoadSceneMode.Single);
    }

    public static void QuitGame()
    {
        Debug.Log("quit game...");
        Application.Quit();
    }
}
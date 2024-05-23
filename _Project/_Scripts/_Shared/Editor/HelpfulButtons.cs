using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SearchService;
using Sirenix.OdinInspector.Editor.Internal;

using System;
using Object = UnityEngine.Object;
public class HelpfulButtons : OdinEditorWindow
{
    [MenuItem("Window/Colnnor/Helpful Buttons")]
    private static void OpenWindow() => GetWindow<HelpfulButtons>().Show();
    [ButtonGroup]
    [Button(ButtonSizes.Large)]
    private void LoadBootstrapper() => LoadScene("Assets/_Project/Scenes/Bootstrapper.unity");
    [ButtonGroup]
    [Button(ButtonSizes.Large)]
    private void LoadMainMenu() => LoadScene("Assets/_Project/Scenes/MainMenu.unity");
    [ButtonGroup]
    [Button(ButtonSizes.Large)]
    private void LoadGameplay() => LoadScene("Assets/_Project/Scenes/Gameplay.unity");

    private void LoadScene(string scenePath)
    {
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }

    [ButtonGroup]
    [Button(ButtonSizes.Large)]
    private void OpenBuildSettings() => EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow, UnityEditor"));

    [Button(ButtonSizes.Large)]
    private void OpenBootstrapper()
    {
        string bootstrapperPath = "Assets/_Project/_Scripts/_Shared/SceneManagement/Bootstrapper.cs";
        Object bootstrapper = AssetDatabase.LoadAssetAtPath<Object>(bootstrapperPath);
        if (bootstrapper != null)
        {
            AssetDatabase.OpenAsset(bootstrapper);
        }
    }
}

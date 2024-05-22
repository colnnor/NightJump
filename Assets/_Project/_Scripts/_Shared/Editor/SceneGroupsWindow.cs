using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
public class SceneGroupsWindow : OdinEditorWindow
{
    [InlineEditor]
    public SceneGroupsSO sceneGroups;

    [MenuItem("Tools/Colnnor/Scene Groups")]
    private static void OpenWindow()
    {
        GetWindow<SceneGroupsWindow>().Show();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        LoadSceneGroups();
    }

    void LoadSceneGroups()
    {
        sceneGroups = AssetDatabase.LoadAssetAtPath<SceneGroupsSO>("Assets/_Project/ScriptableObjects/Tools/SceneGroupsSO.asset");
    }

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


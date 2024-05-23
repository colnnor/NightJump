namespace ulgrude.NotePadFree {
    using UnityEngine;
    using UnityEditor;

    [InitializeOnLoad]
    public class NotePadInitializerFREE {
        static NotePadInitializerFREE() {
            EditorApplication.delayCall += () => {
                if (IsNotePadWindowOpen()) {
                    NotePadWindowFREE.LoadOpenWindow();
                }
            };
        
            EditorApplication.playModeStateChanged += (state) => {
                NotePadWindowFREE[] windows = Resources.FindObjectsOfTypeAll<NotePadWindowFREE>();
                if (state == PlayModeStateChange.ExitingEditMode && windows.Length > 0) {
                    EditorPrefs.SetString("NotePadWindowFilePath", windows[0].filePath);
                }
            };
        }

        static bool IsNotePadWindowOpen() {
            NotePadWindowFREE[] windows = Resources.FindObjectsOfTypeAll<NotePadWindowFREE>();
            return windows.Length > 0;
        }
    }
}
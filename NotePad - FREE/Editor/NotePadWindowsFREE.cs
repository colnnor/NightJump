namespace ulgrude.NotePadFree {
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Collections.Generic;

    public class NotePadWindowFREE : EditorWindow {
        static NotePadWindowFREE windowInstance;

        [MenuItem("Tools/NotePad")]
        public static void OpenWindow() {
            if (windowInstance == null) {
                windowInstance = CreateInstance<NotePadWindowFREE>();
                windowInstance.titleContent = new GUIContent("NotePad");
                windowInstance.Show();
            } else {
                windowInstance.Focus();
            }
        }

        [MenuItem("Assets/Open with NotePad")]
        private static void OpenWithNotePadFREEOption() {
            foreach (var obj in Selection.objects) {
                string path = AssetDatabase.GetAssetPath(obj);
                if (path.EndsWith(".txt")) {
                    OpenWindow();
                    windowInstance.LoadNoteFromFile(path);
                    return;
                }
            }
        }

        [MenuItem("Assets/Open with NotePad", true)]
        private static bool ValidateOpenWithNotePadFREEOption() {
            foreach (var obj in Selection.objects) {
                string path = AssetDatabase.GetAssetPath(obj);
                if (path.EndsWith(".txt")) {
                    return true;
                }
            }
            return false;
        }

        public static void LoadOpenWindow() {
            NotePadWindowFREE[] windows = Resources.FindObjectsOfTypeAll<NotePadWindowFREE>();
            if (windows.Length > 0) {
                if (windows[0].noteContent == "") {
                    windows[0].filePath = EditorPrefs.GetString("NotePadWindowFilePath", windows[0].filePath);
                    windows[0].LoadNote(false);
                }
            }
        }
        
        string noteContent = "";
        public string filePath = "";
        int windowId;
        Vector2 scrollPosition = Vector2.zero;

        Stack<string> undoStack = new Stack<string>();
        Stack<string> redoStack = new Stack<string>();

        void OnDestroy() {
            if (noteContent != "" && (!File.Exists(filePath) || noteContent != File.ReadAllText(filePath))) {
                string noteName = "blank note";
                if (File.Exists(filePath)) {
                    noteName = Path.GetFileName(filePath);
                }

                bool result = EditorUtility.DisplayDialog("Unsaved file", "Do you want to save the " + noteName + " file before exiting?", "Save", "Don't Save");
                if (result) {
                    SaveNote();
                }
            }
        }

        void OnGUI() {
            Event e = Event.current;
            if (e.type == EventType.KeyDown) {
                if (e.modifiers == EventModifiers.Control) {
                    if (e.keyCode == KeyCode.S) {
                        SaveNote();
                        e.Use();
                    } else if (e.keyCode == KeyCode.Z) {
                        Undo();
                        e.Use();
                    } else if (e.keyCode == KeyCode.Y) {
                        Redo();
                        e.Use();
                    } else if (e.keyCode == KeyCode.X || e.keyCode == KeyCode.V) {
                        undoStack.Push(noteContent);
                        redoStack.Clear();
                    }
                } else {
                    float lineHeight = EditorGUIUtility.singleLineHeight;
                    float spaceBetweenLines = EditorGUIUtility.standardVerticalSpacing;
                    if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter) {
                        scrollPosition.x = 0;
                        scrollPosition.y += (lineHeight - spaceBetweenLines * 2);
                    } else if (e.keyCode == KeyCode.UpArrow) {
                        scrollPosition.y -= (lineHeight - spaceBetweenLines * 2);
                    } else if (e.keyCode == KeyCode.DownArrow) {
                        scrollPosition.y += (lineHeight - spaceBetweenLines * 2);
                    } else if (e.keyCode == KeyCode.Home) {
                        scrollPosition.y = 0;
                    } else if (e.keyCode == KeyCode.End) {
                        scrollPosition.y = noteContent.Length * 10;
                    }
                    if (scrollPosition.y < 0) {
                        scrollPosition.y = 0;
                    }
                    
                    if (undoStack.Count == 0 || undoStack.Peek() != noteContent) {
                        undoStack.Push(noteContent);
                        redoStack.Clear();
                    }
                }
            }
            
            Event evt = Event.current;
            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    OnDragAndDrop();
                    break;
            }

            string name = "Blank note (*)";
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) {
                name = Path.GetFileName(filePath);
                if (noteContent != File.ReadAllText(filePath)) {
                    name += " (*)";
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) {
                if (GUILayout.Button("Open File Location", GUILayout.ExpandWidth(false))) {
                    EditorUtility.RevealInFinder(filePath);
                }
            }
            GUILayout.EndHorizontal();


            GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea);
            textAreaStyle.richText = true;
            textAreaStyle.wordWrap = false;
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(position.height - 40));
            noteContent = EditorGUILayout.TextArea(noteContent, textAreaStyle, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath) && noteContent != File.ReadAllText(filePath)) {
                if (GUILayout.Button("Save", GUILayout.ExpandWidth(false))) {
                    SaveNote();
                }
            }

            if (GUILayout.Button("Save as...", GUILayout.ExpandWidth(false))) {
                SaveNote(true);
            }

            if (GUILayout.Button("Load", GUILayout.ExpandWidth(false))) {
                if (noteContent != "" && (!File.Exists(filePath) || noteContent != File.ReadAllText(filePath))) {
                    bool result = EditorUtility.DisplayDialog("Unsaved file", "Are you sure you want to undo the edit and open a new file?", "Load", "Cancel");
                    if (result) {
                        LoadNote();
                        RefreshView();
                    }
                } else {
                    LoadNote();
                    RefreshView();
                }
            }

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath)) {
                if (GUILayout.Button("Open File Location", GUILayout.ExpandWidth(false))) {
                    EditorUtility.RevealInFinder(filePath);
                }
            }

            GUILayout.FlexibleSpace();

            if (noteContent != "" || !string.IsNullOrEmpty(filePath)) {
                if (GUILayout.Button("New blank note", GUILayout.ExpandWidth(false))) {
                    if (noteContent != "" && (!File.Exists(filePath) || noteContent != File.ReadAllText(filePath))) {
                        bool result = EditorUtility.DisplayDialog("Unsaved file", "Are you sure you want to undo the edit and open a blank note?", "New note", "Cancel");
                        if (result) {
                            CloseNote();
                            RefreshView();
                        }
                    } else {
                        CloseNote();
                        RefreshView();
                    }
                }
            }

            GUILayout.EndHorizontal();
        }
        
        void Undo() {
            if (undoStack.Count > 0) {
                redoStack.Push(noteContent);
                noteContent = undoStack.Pop();
                RefreshView();
            }
        }

        void Redo() {
            if (redoStack.Count > 0) {
                undoStack.Push(noteContent);
                noteContent = redoStack.Pop();
                RefreshView();
            }
        }

        void SaveNote(bool openPanel = false) {
            string defaultFileName = "notes.txt";

            string selectedFilePath = "";
            if (string.IsNullOrEmpty(filePath) || openPanel) {
                selectedFilePath = EditorUtility.SaveFilePanel("Save the note", "", defaultFileName, "txt");
                if (!string.IsNullOrEmpty(selectedFilePath)) {
                    filePath = selectedFilePath;
                } else {
                    return;
                }
            }

            if (!string.IsNullOrEmpty(filePath)) {
                File.WriteAllText(filePath, noteContent);
                Debug.Log("Note saved at: " + filePath);
            } else {
                Debug.LogWarning("Invalid file path: " + filePath);
            }
        }

        void LoadNote(bool openPanel = true) {
            string selectedFilePath = "";
            if (openPanel) {
                selectedFilePath = EditorUtility.OpenFilePanel("Load a note", "", "txt");
            } else {
                selectedFilePath = filePath;
            }

            if (!string.IsNullOrEmpty(selectedFilePath)) {
                if (File.Exists(selectedFilePath)) {
                    filePath = selectedFilePath;
                    string[] lines = File.ReadAllLines(filePath);
                    noteContent = string.Join("\n", lines);
                    Debug.Log("Note loaded from: " + filePath);
                } else {
                    Debug.LogWarning("File doesn't exists: " + selectedFilePath);
                }
            }
        }

        void LoadNoteFromFile(string path) {
            filePath = path;
            LoadNote(false);
        }

        void CloseNote() {
            noteContent = "";
            filePath = "";
        }

        void RefreshView() {
            GUIUtility.keyboardControl = 0;
            GUIUtility.hotControl = 0;
        }

        void OnDragAndDrop() {
            Event currentEvent = Event.current;
            EventType eventType = currentEvent.type;

            switch (eventType) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (eventType == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();

                        if (noteContent != "" && (!File.Exists(filePath) || noteContent != File.ReadAllText(filePath))) {
                            bool result = EditorUtility.DisplayDialog("Unsaved file", "Are you sure you want to undo the edit and open a new file?", "Load", "Cancel");
                            if (result) {
                                foreach (string path in DragAndDrop.paths) {
                                    if (File.Exists(path)) {
                                        filePath = path;
                                        LoadNote(false);
                                        break;
                                    }
                                }
                            }
                        } else {
                            foreach (string path in DragAndDrop.paths) {
                                if (File.Exists(path)) {
                                    filePath = path;
                                    LoadNote(false);
                                    break;
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}
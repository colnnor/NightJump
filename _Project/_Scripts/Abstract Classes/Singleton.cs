using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;
    public static bool HasInstance => Instance != null;
    public static T TryGetInstance() => HasInstance ? Instance : null;
    public static T Current => Instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindFirstObjectByType<T>();
                if(instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name + "_AutoCreated";
                    instance = obj.AddComponent<T>();
                }
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    protected virtual void Awake() => InitializeSingleton();

    protected virtual void InitializeSingleton()
    {
        if(!Application.isPlaying) return;

        instance = this as T;
    }
}
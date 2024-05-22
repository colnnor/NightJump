using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    #region Vector3 Extensions
    /// <summary>
    /// Returns a new Vector3 with the specified values, or the original values if not specified
    /// </summary>
    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null) => new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
    public static Vector3Int With(this Vector3Int vector, float? x = null, float? y = null, float? z = null) => new Vector3Int((int)(x ?? vector.x), (int)(y ?? vector.y), (int)(z ?? vector.z));


    public static Vector3Int ToInt(this Vector3 vector) => new Vector3Int(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y), Mathf.CeilToInt(vector.z));
    /// <summary>
    /// Returns a new Vector3 with the specified values added to the original values
    /// </summary>
    public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null) => new Vector3(vector.x + (x ?? 0), vector.y + (y ?? 0), vector.z + (z ?? 0));

    /// <summary>
    /// Randomizes a each value of the Vector3 between the specified min and max values
    /// </summary>
    public static Vector3 Random(this Vector3 vector, float min, float max) => new Vector3(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));

    /// <summary>
    /// Rounds each value of the Vector3 to the nearest whole number
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3 Round(this Vector3 vector) => new (Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));

    public static Vector3 RoundToFloor(this Vector3 vector) => new (Mathf.Floor(vector.x), Mathf.Floor(vector.y), Mathf.Floor(vector.z));

    public static Vector3 RotationTowards(this Vector3 vector, Vector3 target) => vector - target;
    #endregion

    #region Vector2 Extensions
    /// <summary>
    /// Randomizes a each value of the Vector2 between the specified min and max values
    /// </summary>
    public static Vector2 Random(this Vector2 vector, float min, float max) => new Vector2(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));

    #endregion

    #region GameObject Extensions   

    /// <summary>
    /// Gets or adds a component to a GameObject.
    /// </summary>
    /// <returns>The component of type T.</returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (!component) component = gameObject.AddComponent<T>();
        return component;
    }

    /// <summary>
    /// Returns null if the object is null, otherwise returns the object itself.
    /// </summary>
    /// <returns>The object itself if not null, otherwise null.</returns>
    public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

    /// <summary>
    /// Destroys all child objects of the GameObject.
    /// </summary>
    public static void DestroyChildren(this GameObject gameObject)
    {
        gameObject.transform.DestroyChildren();
    }

    /// <summary>
    /// Disables all child objects of the GameObject.
    /// </summary>
    public static void DisableChildren(this GameObject gameObject)
    {
        gameObject.transform.DisableChildren();
    }

    /// <summary>
    /// Enables all child objects of the GameObject.
    /// </summary>
    public static void EnableChildren(this GameObject gameObject)
    {
        gameObject.transform.EnableChildren();
    }
    #endregion


    #region Transform Extensions
    /// <summary>
    /// Returns an enumerable collection of the children of the Transform.
    /// </summary>
    public static IEnumerable<Transform> Children(this Transform parent)
    {
        foreach (Transform child in parent) yield return child;
    }
    public static T GetOrAddComponent<T>(this Transform transform) where T : Component
    {
        return transform.gameObject.GetOrAddComponent<T>();
      
    }
    /// <summary>
    /// Destroys all child objects of the Transform.
    /// </summary>
    public static void DestroyChildren(this Transform parent)
    {
        if (!Application.isPlaying)
        {
            while(parent.childCount > 0)
                Object.DestroyImmediate(parent.GetChild(0).gameObject);

            return;
        }

        parent.PerformActionOnChildren(child => Object.Destroy(child.gameObject));
    }

    /// <summary>
    /// Disables all child objects of the Transform.
    /// </summary>
    public static void DisableChildren(this Transform parent)
    {
        parent.PerformActionOnChildren(child => child.gameObject.SetActive(false));
    }

    /// <summary>
    /// Enables all child objects of the Transform.
    /// </summary>
    public static void EnableChildren(this Transform parent)
    {
        parent.PerformActionOnChildren(child => child.gameObject.SetActive(true));
    }

    private static void PerformActionOnChildren(this Transform parent, System.Action<Transform> action)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            action(parent.GetChild(i));
    }
    #endregion


    #region Float Extensions

    public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        if (toMax > toMin) return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        
        else return toMax + (fromMax - value) * (toMin - toMax) / (fromMax - fromMin);
        
    }

    public static float Random(this float value, float min, float max) => UnityEngine.Random.Range(min, max);


    public static bool Proximity(this float value, float target, float range)
    {
        return Mathf.Abs(value - target) <= range;
    }


    public static float Clamp(this float value, float min, float max)
    {
        return Mathf.Clamp(value, min, max);
    }

    public static float RoundToNearest(this float value, float nearest)
    {
        return Mathf.Round(value / nearest) * nearest;
    }

    public static int ToInt(this float value)
    {
        return Mathf.RoundToInt(value);
    }

    public static float ToPercent(this float value)
    {
        return value * 100;
    }
    #endregion


    #region Bool Extensions

    public static bool Invert(this bool value)
    {
        return !value;
    }

    #endregion


    #region int Extensions

    public static int Clamp(this int value, int min, int max)
    {
        return Mathf.Clamp(value, min, max);
    }
    public static int Squared(this int value) => value * value;
    public static int Random(this int value, int min, int max) => UnityEngine.Random.Range(min, max);
    public static string ToOrdinalString(this int number)
    {
        if (number <= 0)
        {
            Debug.LogError("ToOrdinalString() requires a positive integer.");
            return number.ToString();
        }
        switch (number % 100)
        {
            case 11:
            case 12:
            case 13:
                return number + "th";
        }

        switch (number % 10)
        {
            case 1:
                return number + "st";
            case 2:
                return number + "nd";
            case 3:
                return number + "rd";
            default:
                return number + "th";
        }
    }
    public static bool IsPositive(this int value) => value > 0;
    public static bool IsEven(this int value) => value % 2 == 0;

    #endregion

    public static void Shuffle<T>(this List<T> list)
    {
        System.Random rng = new();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
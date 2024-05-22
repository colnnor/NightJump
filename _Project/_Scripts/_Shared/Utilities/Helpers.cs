using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;

public static class Helpers
{
    private static Camera camera;
    /// <summary>
    /// Gets the main camera in the scene.
    /// </summary>
    /// <returns>The main camera.</returns>
    public static Camera Camera
    {
        get
        {
            if (camera == null) camera = Camera.main;
            return camera;
        }
    }

    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new();
    /// <summary>
    /// If the provided WaitForSeconds already exists, it will return the existing object. Otherwise, it will create a new one.
    /// </summary>
    /// <param name="time">The time to wait.</param>
    /// <returns>The WaitForSeconds object.</returns>
    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out WaitForSeconds wait)) return wait;

        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }

    private static PointerEventData eventDataCurrentPosition;
    private static List<RaycastResult> results;
    /// <summary>
    /// Checks if the mouse is over any UI element.
    /// </summary>
    /// <returns>True if the mouse is over a UI element, false otherwise.</returns>
    public static bool IsOverUI()
    {
        eventDataCurrentPosition = new(EventSystem.current) { position = Input.mousePosition };
        results = new();

        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    /// <summary>
    /// Gets the world real position of a canvas UI element.
    /// </summary>
    /// <param name="element">The RectTransform of the UI element.</param>
    /// <returns>The world position of the UI element.</returns>
    public static Vector2 GetWorldPositionOffCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
        return result;
    }


    /// <summary>
    /// Creates a world text object at the given location
    /// </summary>
    public static TextMeshPro CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 11, Color? color = null, TextAlignmentOptions textAlignment = TextAlignmentOptions.Midline, int sortingOrder = 5000)
    {
        if (color == null) color = Color.white;
        return CreateWorldTextObject(parent, text, localPosition, fontSize, (Color)color, textAlignment, sortingOrder);
    }
    /// <summary>
    /// Creates a world text object at the given location
    /// </summary>
    public static TextMeshPro CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), Color? color = null, int fontSize = 11)
    {
        TextAlignmentOptions textAlignment = TextAlignmentOptions.Midline;
        int sortingOrder = 5000;
        if (color == null) color = Color.white;
        return CreateWorldTextObject(parent, text, localPosition, fontSize, (Color)color, textAlignment, sortingOrder);
    }

    private static TextMeshPro CreateWorldTextObject(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAlignmentOptions textAlignment, int sortingOrder)
    {
        GameObject textObject = new("World_Text", typeof(TextMeshPro));
        Transform transform = textObject.transform;
        MeshRenderer renderer = textObject.GetComponent<MeshRenderer>();
        renderer.sortingOrder = sortingOrder;
        transform.SetParent(parent);
        transform.localPosition = localPosition;
        TextMeshPro textMesh = textObject.GetComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.color = color;
        textMesh.alignment = textAlignment;
        textMesh.fontSize = fontSize;
        return textMesh;
    }

    /// <summary>
    /// Returns the ceiling of a random range between min and max
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int GetRandomCeil(float min, float max)
    {
        return Mathf.RoundToInt(Random.Range(min, max));
    }
}

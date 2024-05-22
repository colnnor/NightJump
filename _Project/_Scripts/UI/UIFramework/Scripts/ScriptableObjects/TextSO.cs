using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "TextSO", menuName = "CustomUI/TextSO", order = 2)]
[InlineEditor]
public class TextSO : ScriptableObject
{
    public TMP_FontAsset font;
    public float size;
}

using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Text : CustomUIComponent
{
    [Title("Theme Data")]
    [ColorPalette("Menu Theme")]
    [BoxGroup("Theme")]
    [SerializeField] private Color palette = new(0.95f, 0.95f, 0.95f, 1f);

    [BoxGroup("Theme")]
    [SerializeField] private Color textColor = new(0,0,0,255);
    [BoxGroup("Theme")]
    [SerializeField] private Color underlayColor = new(0, 0, 0, 255);


    [Title("View Settings")]
    [BoxGroup("Text Data", ShowLabel = false)]
    [SerializeField] private string textString = "New Text";
    [BoxGroup("Text Data", ShowLabel = false)]
    [SerializeField] private bool overrideFontSize = false;
    [BoxGroup("Text Data", ShowLabel = false)]
    [ShowIf("overrideFontSize")]
    [SerializeField] private int fontSize = 100;
    [BoxGroup("Text Data", ShowLabel = false)]
    [SerializeField] private TextSO textData;

    public bool showDistance = false;

    private TextMeshProUGUI text;

    public override void Setup() => text = GetComponentInChildren<TextMeshProUGUI>();
    public override void Configure()
    {
        if (!textData)
        {
            Debug.Log("No data or theme assigned...");
            return;
        }
        text.text = textString;
        text.font = textData.font;
        text.fontSize = overrideFontSize? fontSize : textData.size;
        text.color = textColor;
        text.material.SetColor("_UnderlayColor", underlayColor);
    }

    public void UpdateText(string newText) => text.text = newText;
}
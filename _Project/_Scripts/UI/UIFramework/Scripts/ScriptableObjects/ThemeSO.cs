using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ThemeSO", menuName = "CustomUI/ThemeSO", order = 1)]
public class ThemeSO : ScriptableObject
{

    [ColorPalette("Menu Theme")]
    public Color primaryColorBG = new(0.95f, 0.95f, 0.95f, 1f); 
/*
    public Color GetBGColor(UIStyle style)
    {
        return style switch
        {
            UIStyle.Primary => primaryColorBG,
            UIStyle.Secondary => secondaryColorBG,
            UIStyle.Tertiary => tertiaryColorBG,
            _ => disabled
        };
    }
    public Color GetTextColor(UIStyle style)
    {
        return style switch
        {
            UIStyle.Primary => primaryColorText,
            UIStyle.Secondary => secondaryColorText,
            UIStyle.Tertiary => tertiaryColorText,
            _ => disabled
        };
    }

    public Color GetUnderlayColor(UIStyle style)
    {
        return style switch
        {
            UIStyle.Primary => primaryTextUnderlayColor,
            UIStyle.Secondary => secondaryTextUnderlayColor,
            UIStyle.Tertiary => tertiaryTextUnderlayColor,
            _ => disabled
        };
    }*/
}

using System;
using UnityEditor;
using UnityEngine;

public static class IconKeyedOptionPicker
{
    public static bool DrawIconOnlyButton(
        IconCache iconCache,
        string iconAssetName,
        float width,
        float height
    )
    {
        var icon = iconCache.GetIcon(iconAssetName);
        return GUILayout.Button(icon, GUILayout.Width(width), GUILayout.Height(height));
    }

    public static bool DrawTextWithIconButton(
        IconCache iconCache,
        string text,
        string iconAssetName,
        float width,
        float height,
        bool showText = true
    )
    {
        var icon = iconCache.GetIcon(iconAssetName);
        if (!showText)
            return GUILayout.Button(icon, GUILayout.Width(width), GUILayout.Height(height));
        return GUILayout.Button(
            new GUIContent(text, icon),
            GUILayout.Width(width),
            GUILayout.Height(height)
        );
    }

    public static void Show(
        string title,
        string emptyMessage,
        string[] optionValues,
        string[] optionIconFileNames,
        Action<string> onSelected,
        Vector2 screenPosition,
        string iconsFolderPath,
        bool allowNone = false
    )
    {
        if (optionValues.Length != optionIconFileNames.Length)
            throw new ArgumentException("optionValues and optionIconFileNames length must match.");

        SelectorPopup.Show(
            title,
            emptyMessage,
            optionValues,
            onSelected,
            screenPosition,
            iconsFolderPath,
            allowNone,
            optionIconFileNames
        );
    }

    public static void ShowWhenIconFileNameMatchesValue(
        string title,
        string emptyMessage,
        string[] optionValues,
        Action<string> onSelected,
        Vector2 screenPosition,
        string iconsFolderPath,
        bool allowNone = false
    )
    {
        Show(
            title,
            emptyMessage,
            optionValues,
            optionValues,
            onSelected,
            screenPosition,
            iconsFolderPath,
            allowNone
        );
    }
}

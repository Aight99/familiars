using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PopupListSearch
{
    private const string searchLabel = "Поиск";

    public static string DrawField(string query)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(searchLabel, GUILayout.Width(48f));
        var next = EditorGUILayout.TextField(string.IsNullOrEmpty(query) ? "" : query);
        EditorGUILayout.EndHorizontal();
        return next;
    }

    public static bool Matches(string displayText, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return true;
        if (string.IsNullOrEmpty(displayText))
            return false;
        return displayText.IndexOf(query.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool MatchesIconAssetFileName(string fileNameWithExtension, string query)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
            return false;
        var withoutExt = Path.GetFileNameWithoutExtension(fileNameWithExtension);
        return Matches(withoutExt, query) || Matches(fileNameWithExtension, query);
    }
}

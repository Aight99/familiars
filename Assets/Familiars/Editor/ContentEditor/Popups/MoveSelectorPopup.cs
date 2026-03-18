using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveSelectorPopup : EditorWindow
{
    private static readonly string noneLabel = "(none)";

    private string[] options = Array.Empty<string>();
    private int selectedIndex;
    private Action<string> onChange;

    public static void Show(
        List<MoveEntry> moves,
        string currentValue,
        Action<string> onChange,
        Vector2 screenPosition
    )
    {
        var window = CreateInstance<MoveSelectorPopup>();
        window.titleContent = new GUIContent("Select Move");
        window.options = BuildOptions(moves);
        window.selectedIndex = FindIndex(window.options, currentValue);
        window.onChange = onChange;
        window.ShowUtility();
        window.position = new Rect(screenPosition.x, screenPosition.y, 220, 50);
    }

    private void OnGUI()
    {
        if (options.Length == 0)
        {
            EditorGUILayout.LabelField("No moves available. Sync Move editor first.");
            return;
        }

        var newIndex = EditorGUILayout.Popup("Move", selectedIndex, options);
        if (newIndex != selectedIndex)
        {
            selectedIndex = newIndex;
            onChange?.Invoke(options[selectedIndex] == noneLabel ? "" : options[selectedIndex]);
        }
    }

    private static string[] BuildOptions(List<MoveEntry> moves)
    {
        if (moves == null || moves.Count == 0)
            return new[] { noneLabel };

        var options = new string[moves.Count + 1];
        options[0] = noneLabel;
        for (var i = 0; i < moves.Count; i++)
            options[i + 1] = moves[i].name;
        return options;
    }

    private static int FindIndex(string[] options, string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;
        for (var i = 0; i < options.Length; i++)
        {
            if (options[i] == value)
                return i;
        }
        return 0;
    }
}

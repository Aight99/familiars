using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TypeSelectorPopup : EditorWindow
{
    private string[] typeNames = Array.Empty<string>();
    private int selectedIndex;
    private Action<string> onChange;

    public static void Show(
        List<TypeElementEntry> types,
        string currentValue,
        Action<string> onChange,
        Vector2 screenPosition
    )
    {
        var window = CreateInstance<TypeSelectorPopup>();
        window.titleContent = new GUIContent("Select Type");
        window.typeNames = BuildTypeNames(types);
        window.selectedIndex = FindIndex(window.typeNames, currentValue);
        window.onChange = onChange;
        window.ShowUtility();
        window.position = new Rect(screenPosition.x, screenPosition.y, 220, 50);
    }

    private void OnGUI()
    {
        if (typeNames.Length == 0)
        {
            EditorGUILayout.LabelField("No types available. Sync TypeElement editor first.");
            return;
        }

        var newIndex = EditorGUILayout.Popup("Type", selectedIndex, typeNames);
        if (newIndex != selectedIndex)
        {
            selectedIndex = newIndex;
            onChange?.Invoke(typeNames[selectedIndex]);
        }
    }

    private static string[] BuildTypeNames(List<TypeElementEntry> types)
    {
        if (types == null || types.Count == 0)
            return Array.Empty<string>();

        var names = new string[types.Count];
        for (var i = 0; i < types.Count; i++)
            names[i] = types[i].name;
        return names;
    }

    private static int FindIndex(string[] names, string value)
    {
        for (var i = 0; i < names.Length; i++)
        {
            if (names[i] == value)
                return i;
        }
        return 0;
    }
}

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TypeListEditPopup : EditorWindow
{
    private string windowTitle = "";
    private List<string> items = new();
    private string[] typeNames = Array.Empty<string>();
    private Action<string[]> onChange;
    private Vector2 scrollPosition;

    public static void Show(
        string title,
        string[] currentItems,
        List<TypeElementEntry> allTypes,
        Action<string[]> onChange,
        Vector2 screenPosition
    )
    {
        var window = CreateInstance<TypeListEditPopup>();
        window.titleContent = new GUIContent(title);
        window.windowTitle = title;
        window.items = new List<string>(currentItems ?? Array.Empty<string>());
        window.typeNames = SelectorPopupUtils.BuildNames(allTypes, t => t.name);
        window.onChange = onChange;
        window.ShowUtility();
        window.position = new Rect(screenPosition.x, screenPosition.y, 240, 200);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(windowTitle, EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(
            scrollPosition,
            GUILayout.ExpandHeight(true)
        );

        var changed = false;
        for (var i = 0; i < items.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            var currentIndex = SelectorPopupUtils.FindIndex(typeNames, items[i]);
            var newIndex = EditorGUILayout.Popup(currentIndex, typeNames);
            if (newIndex != currentIndex && typeNames.Length > 0)
            {
                items[i] = typeNames[newIndex];
                changed = true;
            }

            if (GUILayout.Button("-", GUILayout.Width(24)))
            {
                items.RemoveAt(i);
                changed = true;
                EditorGUILayout.EndHorizontal();
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("+ Add"))
        {
            items.Add(typeNames.Length > 0 ? typeNames[0] : "");
            changed = true;
        }

        if (changed)
            onChange?.Invoke(items.ToArray());
    }
}

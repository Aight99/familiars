using System;
using UnityEditor;
using UnityEngine;

public class TextEditPopup : EditorWindow
{
    private string windowTitle = "";
    private string value = "";
    private Action<string> onChange;

    public static void Show(
        string title,
        string currentValue,
        Action<string> onChange,
        Vector2 screenPosition
    )
    {
        var window = CreateInstance<TextEditPopup>();
        window.titleContent = new GUIContent(title);
        window.windowTitle = title;
        window.value = currentValue;
        window.onChange = onChange;
        window.ShowUtility();
        window.position = new Rect(screenPosition.x, screenPosition.y, 280, 60);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(windowTitle, EditorStyles.boldLabel);
        var newValue = EditorGUILayout.TextField(value);
        if (newValue != value)
        {
            value = newValue;
            onChange?.Invoke(value);
        }
    }
}

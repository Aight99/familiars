using System;
using UnityEditor;
using UnityEngine;

public class IntEditPopup : EditorWindow
{
    private string windowTitle = "";
    private int value;
    private Action<int> onChange;

    public static void Show(
        string title,
        int currentValue,
        Action<int> onChange,
        Vector2 screenPosition
    )
    {
        var window = CreateInstance<IntEditPopup>();
        window.titleContent = new GUIContent(title);
        window.windowTitle = title;
        window.value = currentValue;
        window.onChange = onChange;
        window.ShowUtility();
        window.position = new Rect(screenPosition.x, screenPosition.y, 200, 60);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(windowTitle, EditorStyles.boldLabel);
        var newValue = EditorGUILayout.IntField(value);
        if (newValue != value)
        {
            value = newValue;
            onChange?.Invoke(value);
        }
    }
}

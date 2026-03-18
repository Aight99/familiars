using System;
using UnityEditor;
using UnityEngine;

public class SelectorPopup : EditorWindow
{
    private static readonly string noneLabel = "(none)";
    private static readonly float buttonSize = 64f;
    private static readonly float labelHeight = 16f;
    private static readonly int maxColumns = 4;

    private string emptyMessage;
    private string[] options = Array.Empty<string>();
    private Texture2D[] icons;
    private bool allowNone;
    private Action<string> onChange;

    public static void Show(
        string title,
        string emptyMessage,
        string[] options,
        Action<string> onChange,
        Vector2 screenPosition,
        string iconsFolderPath = null,
        bool allowNone = false
    )
    {
        var window = CreateInstance<SelectorPopup>();
        window.titleContent = new GUIContent(title);
        window.emptyMessage = emptyMessage;
        window.allowNone = allowNone;
        window.options = allowNone ? Prepend(noneLabel, options) : options;
        window.onChange = onChange;
        window.icons = LoadIcons(window.options, iconsFolderPath);
        window.ShowUtility();
        window.position = new Rect(
            screenPosition.x,
            screenPosition.y,
            CalcWidth(window.options.Length),
            CalcHeight(window.options.Length)
        );
    }

    private void OnGUI()
    {
        var realCount = options.Length - (allowNone ? 1 : 0);
        if (realCount == 0)
        {
            EditorGUILayout.LabelField(emptyMessage);
            return;
        }

        var cols = Mathf.Min(options.Length, maxColumns);
        for (var i = 0; i < options.Length; i++)
        {
            if (i % cols == 0)
            {
                if (i > 0)
                    EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
            DrawOptionButton(i);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawOptionButton(int i)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(buttonSize + 4f));
        var content = icons[i] != null ? new GUIContent(icons[i]) : new GUIContent(options[i]);
        if (GUILayout.Button(content, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
        {
            onChange?.Invoke(allowNone && options[i] == noneLabel ? "" : options[i]);
            Close();
        }
        EditorGUILayout.LabelField(
            options[i],
            EditorStyles.centeredGreyMiniLabel,
            GUILayout.Width(buttonSize)
        );
        EditorGUILayout.EndVertical();
    }

    private static Texture2D[] LoadIcons(string[] options, string folderPath)
    {
        var result = new Texture2D[options.Length];
        if (string.IsNullOrEmpty(folderPath))
            return result;
        for (var i = 0; i < options.Length; i++)
        {
            var path = $"{folderPath}/{options[i]}.png";
            result[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        return result;
    }

    private static float CalcWidth(int count)
    {
        if (count == 0)
            return 220f;
        var cols = Mathf.Min(count, maxColumns);
        return cols * (buttonSize + 4f) + 4f;
    }

    private static float CalcHeight(int count)
    {
        if (count == 0)
            return 40f;
        var cols = Mathf.Min(count, maxColumns);
        var rows = Mathf.CeilToInt((float)count / cols);
        return rows * (buttonSize + labelHeight + 4f) + 4f;
    }

    private static string[] Prepend(string first, string[] rest)
    {
        var result = new string[rest.Length + 1];
        result[0] = first;
        for (var i = 0; i < rest.Length; i++)
            result[i + 1] = rest[i];
        return result;
    }
}

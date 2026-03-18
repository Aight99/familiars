using System;
using UnityEditor;
using UnityEngine;

public class MoveApplicationTypeSelectorPopup : EditorWindow
{
    private static readonly string[] typeNames = { "Physical", "Ranged", "Status" };
    private static readonly float buttonSize = 64f;

    private Texture2D[] icons;
    private Action<string> onChange;

    public static void Show(Action<string> onChange, Vector2 screenPosition)
    {
        var window = CreateInstance<MoveApplicationTypeSelectorPopup>();
        window.titleContent = new GUIContent("Select Application Type");
        window.onChange = onChange;
        window.LoadIcons();
        window.ShowUtility();
        window.position = new Rect(screenPosition.x, screenPosition.y, 220, 90);
    }

    private void LoadIcons()
    {
        icons = new Texture2D[typeNames.Length];
        for (var i = 0; i < typeNames.Length; i++)
        {
            var path =
                $"{ContentEditorConfig.MoveApplicationTypeIconsFolderPath}/{typeNames[i]}.png";
            icons[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        for (var i = 0; i < typeNames.Length; i++)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(buttonSize + 4f));
            var content =
                icons[i] != null ? new GUIContent(icons[i]) : new GUIContent(typeNames[i]);
            if (
                GUILayout.Button(content, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize))
            )
            {
                onChange?.Invoke(typeNames[i]);
                Close();
            }
            EditorGUILayout.LabelField(
                typeNames[i],
                EditorStyles.centeredGreyMiniLabel,
                GUILayout.Width(buttonSize)
            );
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }
}

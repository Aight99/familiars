using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class IconPickerPopup : EditorWindow
{
    private static readonly float cellSize = 64f;
    private static readonly float cellPadding = 4f;
    private static readonly float labelHeight = 16f;

    private string[] iconNames = Array.Empty<string>();
    private Texture2D[] textures = Array.Empty<Texture2D>();
    private Action<string> onChange;
    private Vector2 scrollPosition;
    private string currentSelection = "";

    public static void Show(string folderPath, string currentIconName, Action<string> onChange, Vector2 screenPosition)
    {
        var window = CreateInstance<IconPickerPopup>();
        window.titleContent = new GUIContent("Select Icon");
        window.onChange = onChange;
        window.currentSelection = currentIconName;
        window.LoadIcons(folderPath);
        window.ShowUtility();
        window.position = new Rect(screenPosition.x, screenPosition.y, 320, 400);
    }

    private void LoadIcons(string folderPath)
    {
        var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
        iconNames = new string[guids.Length];
        textures = new Texture2D[guids.Length];

        for (var i = 0; i < guids.Length; i++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            iconNames[i] = Path.GetFileName(assetPath);
            textures[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        }
    }

    private void OnGUI()
    {
        if (iconNames.Length == 0)
        {
            EditorGUILayout.HelpBox("В папке нет изображений.", MessageType.Info);
            return;
        }

        var windowWidth = position.width - 16f;
        var columns = Mathf.Max(1, Mathf.FloorToInt(windowWidth / (cellSize + cellPadding)));
        var cellTotal = cellSize + cellPadding + labelHeight;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (var i = 0; i < iconNames.Length; i++)
        {
            if (i % columns == 0)
                EditorGUILayout.BeginHorizontal();

            DrawCell(i);

            if (i % columns == columns - 1 || i == iconNames.Length - 1)
                EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawCell(int index)
    {
        var name = iconNames[index];
        var texture = textures[index];
        var isSelected = name == currentSelection;

        EditorGUILayout.BeginVertical(GUILayout.Width(cellSize + cellPadding));

        var style = new GUIStyle(GUI.skin.button)
        {
            padding = new RectOffset(2, 2, 2, 2),
            normal = { background = isSelected ? Texture2D.grayTexture : null },
        };

        if (GUILayout.Button(texture, style, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
        {
            currentSelection = name;
            onChange?.Invoke(name);
            Close();
        }

        var labelStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
        {
            wordWrap = false,
        };
        EditorGUILayout.LabelField(Path.GetFileNameWithoutExtension(name), labelStyle, GUILayout.Width(cellSize));

        EditorGUILayout.EndVertical();
    }
}

using System;
using UnityEditor;
using UnityEngine;

public class SelectorPopup : EditorWindow
{
    private static readonly string noneLabel = "(none)";
    private static readonly float cellSize = 64f;
    private static readonly float cellPadding = 4f;
    private static readonly float iconGridWindowWidth = 320f;
    private static readonly float iconGridWindowHeight = 400f;
    private static readonly float tableWindowWidth = 260f;
    private static readonly float maxWindowHeight = 440f;
    private static readonly float maxScrollBodyHeight = 400f;
    private static readonly float tableVerticalPadding = 12f;

    private string emptyMessage;
    private string[] options = Array.Empty<string>();
    private Texture2D[] icons;
    private bool allowNone;
    private bool useIconGrid;
    private Action<string> onChange;
    private Vector2 scrollPosition;

    public static void Show(
        string title,
        string emptyMessage,
        string[] options,
        Action<string> onChange,
        Vector2 screenPosition,
        string iconsFolderPath = null,
        bool allowNone = false,
        string[] optionIconFileNames = null
    )
    {
        var window = CreateInstance<SelectorPopup>();
        window.titleContent = new GUIContent(title);
        window.emptyMessage = emptyMessage;
        window.allowNone = allowNone;
        window.options = allowNone ? Prepend(noneLabel, options) : options;
        window.onChange = onChange;
        var iconKeysForLoad = BuildIconKeysForLoad(allowNone, optionIconFileNames, options.Length);
        window.icons = LoadIcons(window.options, iconsFolderPath, iconKeysForLoad);
        window.useIconGrid =
            !string.IsNullOrEmpty(iconsFolderPath) && HasAnyLoadedIcon(window.icons);
        window.ShowUtility();
        var count = window.options.Length;
        var width = window.useIconGrid ? iconGridWindowWidth : tableWindowWidth;
        var height = window.useIconGrid
            ? iconGridWindowHeight
            : Mathf.Min(CalcTableHeight(count), maxWindowHeight);
        window.position = new Rect(screenPosition.x, screenPosition.y, width, height);
    }

    private void OnGUI()
    {
        var realCount = options.Length - (allowNone ? 1 : 0);
        if (realCount == 0)
        {
            EditorGUILayout.LabelField(emptyMessage);
            return;
        }

        if (useIconGrid)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawIconGrid();
            EditorGUILayout.EndScrollView();
        }
        else
        {
            var bodyHeight = Mathf.Min(CalcTableContentHeight(options.Length), maxScrollBodyHeight);
            scrollPosition = EditorGUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.Height(bodyHeight)
            );
            DrawTable();
            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawIconGrid()
    {
        var windowWidth = position.width - 16f;
        var columns = Mathf.Max(1, Mathf.FloorToInt(windowWidth / (cellSize + cellPadding)));
        for (var i = 0; i < options.Length; i++)
        {
            if (i % columns == 0)
                EditorGUILayout.BeginHorizontal();

            DrawIconCell(i);

            if (i % columns == columns - 1 || i == options.Length - 1)
                EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawIconCell(int i)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(cellSize + cellPadding));

        var style = new GUIStyle(GUI.skin.button) { padding = new RectOffset(2, 2, 2, 2) };

        var texture = icons[i];
        if (texture != null)
        {
            if (
                GUILayout.Button(
                    texture,
                    style,
                    GUILayout.Width(cellSize),
                    GUILayout.Height(cellSize)
                )
            )
                InvokeSelect(i);
        }
        else
        {
            if (
                GUILayout.Button(
                    GUIContent.none,
                    style,
                    GUILayout.Width(cellSize),
                    GUILayout.Height(cellSize)
                )
            )
                InvokeSelect(i);
        }

        var labelStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { wordWrap = false };
        EditorGUILayout.LabelField(options[i], labelStyle, GUILayout.Width(cellSize));

        EditorGUILayout.EndVertical();
    }

    private void DrawTable()
    {
        var rowHeight = EditorGUIUtility.singleLineHeight + 4f;
        for (var i = 0; i < options.Length; i++)
        {
            if (
                GUILayout.Button(
                    options[i],
                    GUILayout.ExpandWidth(true),
                    GUILayout.Height(rowHeight)
                )
            )
                InvokeSelect(i);
        }
    }

    private void InvokeSelect(int i)
    {
        onChange?.Invoke(allowNone && options[i] == noneLabel ? "" : options[i]);
        Close();
    }

    private static string[] BuildIconKeysForLoad(
        bool allowNone,
        string[] optionIconFileNames,
        int baseOptionCount
    )
    {
        if (optionIconFileNames == null)
            return null;
        if (optionIconFileNames.Length != baseOptionCount)
            throw new ArgumentException(
                "optionIconFileNames length must match options length.",
                nameof(optionIconFileNames)
            );
        return allowNone ? Prepend("", optionIconFileNames) : optionIconFileNames;
    }

    private static Texture2D[] LoadIcons(
        string[] optionList,
        string folderPath,
        string[] iconFileNamesPerRow
    )
    {
        var result = new Texture2D[optionList.Length];
        if (string.IsNullOrEmpty(folderPath))
            return result;
        if (iconFileNamesPerRow == null)
        {
            for (var i = 0; i < optionList.Length; i++)
            {
                var path = $"{folderPath}/{optionList[i]}.png";
                result[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
            return result;
        }

        var loader = new IconCache(folderPath);
        for (var i = 0; i < optionList.Length; i++)
            result[i] = loader.LoadTextureOrNull(iconFileNamesPerRow[i]);
        return result;
    }

    private static bool HasAnyLoadedIcon(Texture2D[] textures)
    {
        for (var i = 0; i < textures.Length; i++)
        {
            if (textures[i] != null)
                return true;
        }
        return false;
    }

    private static float CalcTableHeight(int count)
    {
        if (count == 0)
            return 40f;
        return CalcTableContentHeight(count) + tableVerticalPadding;
    }

    private static float CalcTableContentHeight(int count)
    {
        var rowHeight = EditorGUIUtility.singleLineHeight + 4f;
        return count * rowHeight + 4f;
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

using UnityEditor;
using UnityEngine;

public abstract class ContentEditorWindow : EditorWindow
{
    protected static readonly float rowHeight = 40f;
    protected static readonly float colNum = 30f;
    protected static readonly float colIcon = 50f;

    private IconCache iconCacheInstance;
    private Vector2 scrollPosition;

    protected IconCache iconCache => iconCacheInstance ??= new IconCache(IconsFolderPath);

    protected abstract string WindowTitle { get; }
    protected abstract string IconsFolderPath { get; }
    protected abstract int EntryCount { get; }

    protected abstract void OnSync();
    protected abstract void OnExport();
    protected abstract void OnAddEntry();
    protected abstract void OnRemoveLastEntry();
    protected abstract void DrawColumnHeaders();
    protected abstract void DrawRow(int index);

    private void OnEnable() => Sync();

    private void Sync()
    {
        OnSync();
        iconCache.Clear();
    }

    private void Export()
    {
        OnExport();
        AssetDatabase.Refresh();
    }

    private void OnGUI()
    {
        DrawToolbar();
        DrawHeaderRow();
        DrawTable();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Export", EditorStyles.toolbarButton, GUILayout.Width(60)))
            Export();

        if (GUILayout.Button("Sync", EditorStyles.toolbarButton, GUILayout.Width(50)))
        {
            if (
                EditorUtility.DisplayDialog(
                    $"Sync {WindowTitle}",
                    "Все несохранённые изменения будут потеряны. Продолжить?",
                    "Синхронизировать",
                    "Отмена"
                )
            )
            {
                Sync();
            }
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(24)))
            OnAddEntry();

        if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(24)))
            OnRemoveLastEntry();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawHeaderRow()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("#", GUILayout.Width(colNum));
        GUILayout.Label("Icon", GUILayout.Width(colIcon));
        DrawColumnHeaders();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTable()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (var i = 0; i < EntryCount; i++)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Height(rowHeight));
            GUILayout.Label(
                (i + 1).ToString(),
                GUILayout.Width(colNum),
                GUILayout.Height(rowHeight)
            );
            DrawRow(i);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    protected void DrawIconCell(string iconName, System.Action<string> onChanged)
    {
        var icon = iconCache.GetIcon(iconName);
        if (GUILayout.Button(icon, GUILayout.Width(colIcon), GUILayout.Height(rowHeight)))
        {
            IconPickerPopup.Show(
                IconsFolderPath,
                iconName,
                v =>
                {
                    onChanged(v);
                    iconCache.Clear();
                    Repaint();
                },
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition)
            );
        }
    }

    protected void DrawTextCell(
        string value,
        float width,
        string popupTitle,
        System.Action<string> onChanged
    )
    {
        if (GUILayout.Button(value, GUILayout.Width(width), GUILayout.Height(rowHeight)))
        {
            TextEditPopup.Show(
                popupTitle,
                value,
                v =>
                {
                    onChanged(v);
                    Repaint();
                },
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition)
            );
        }
    }
}

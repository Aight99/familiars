using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TypeElementEditorWindow : EditorWindow
{
    private static readonly float rowHeight = 40f;
    private static readonly float colNum = 30f;
    private static readonly float colIcon = 50f;
    private static readonly float colName = 120f;
    private static readonly float colEffective = 160f;
    private static readonly float colIneffective = 160f;

    private List<TypeElementEntry> entries = new();
    private readonly IconCache iconCache = new();
    private Vector2 scrollPosition;

    [MenuItem("Window/Familiars/TypeElement Editor")]
    public static void Open()
    {
        var window = GetWindow<TypeElementEditorWindow>("TypeElement Editor");
        window.minSize = new Vector2(600, 300);
        window.Sync();
    }

    private void OnEnable()
    {
        Sync();
    }

    private void Sync()
    {
        entries = JsonDataService.Load<TypeElementEntry>(ContentEditorConfig.TypeElementFileName);
        iconCache.Clear();
        Debug.Log($"TypeElementEditor: synced {entries.Count} entries.");
    }

    private void Export()
    {
        JsonDataService.Save(ContentEditorConfig.TypeElementFileName, entries);
        AssetDatabase.Refresh();
    }

    private void OnGUI()
    {
        DrawToolbar();
        DrawColumnHeaders();
        DrawTable();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Export", EditorStyles.toolbarButton, GUILayout.Width(60)))
            Export();

        if (GUILayout.Button("Sync", EditorStyles.toolbarButton, GUILayout.Width(50)))
            Sync();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            entries.Add(new TypeElementEntry());
            Debug.Log("TypeElementEditor: added new entry.");
        }

        if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            if (entries.Count > 0)
            {
                entries.RemoveAt(entries.Count - 1);
                Debug.Log("TypeElementEditor: removed last entry.");
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawColumnHeaders()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("#", GUILayout.Width(colNum));
        GUILayout.Label("Icon", GUILayout.Width(colIcon));
        GUILayout.Label("Name", GUILayout.Width(colName));
        GUILayout.Label("Effective Against", GUILayout.Width(colEffective));
        GUILayout.Label("Ineffective Against", GUILayout.Width(colIneffective));
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTable()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (var i = 0; i < entries.Count; i++)
            DrawRow(i);

        EditorGUILayout.EndScrollView();
    }

    private void DrawRow(int index)
    {
        var entry = entries[index];
        EditorGUILayout.BeginHorizontal(GUILayout.Height(rowHeight));

        GUILayout.Label(
            (index + 1).ToString(),
            GUILayout.Width(colNum),
            GUILayout.Height(rowHeight)
        );

        DrawIconCell(entry, index);
        DrawTextCell(entry.name, colName, "Edit Name", v => entries[index].name = v);
        DrawTypeListCell(
            entry.effectiveAgainst,
            colEffective,
            "Effective Against",
            v => entries[index].effectiveAgainst = v
        );
        DrawTypeListCell(
            entry.ineffectiveAgainst,
            colIneffective,
            "Ineffective Against",
            v => entries[index].ineffectiveAgainst = v
        );

        EditorGUILayout.EndHorizontal();
    }

    private void DrawIconCell(TypeElementEntry entry, int index)
    {
        var icon = iconCache.GetIcon(entry.icon);
        if (GUILayout.Button(icon, GUILayout.Width(colIcon), GUILayout.Height(rowHeight)))
        {
            TextEditPopup.Show(
                "Edit Icon",
                entry.icon,
                v =>
                {
                    entries[index].icon = v;
                    iconCache.Clear();
                    Repaint();
                },
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition)
            );
        }
    }

    private void DrawTextCell(
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

    private void DrawTypeListCell(
        string[] items,
        float width,
        string popupTitle,
        System.Action<string[]> onChanged
    )
    {
        var label = items != null && items.Length > 0 ? string.Join(", ", items) : "(none)";
        if (GUILayout.Button(label, GUILayout.Width(width), GUILayout.Height(rowHeight)))
        {
            TypeListEditPopup.Show(
                popupTitle,
                items,
                entries,
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

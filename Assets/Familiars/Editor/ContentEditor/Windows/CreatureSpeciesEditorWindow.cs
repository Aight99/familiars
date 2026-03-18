using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreatureSpeciesEditorWindow : EditorWindow
{
    private static readonly float rowHeight = 40f;
    private static readonly float colNum = 30f;
    private static readonly float colIcon = 50f;
    private static readonly float colName = 140f;
    private static readonly float colType = 100f;
    private static readonly float colStats = 160f;

    private List<CreatureSpeciesEntry> entries = new();
    private List<TypeElementEntry> typeEntries = new();
    private readonly IconCache iconCache = new();
    private Vector2 scrollPosition;

    [MenuItem("Window/Familiars/CreatureSpecies Editor")]
    public static void Open()
    {
        var window = GetWindow<CreatureSpeciesEditorWindow>("CreatureSpecies Editor");
        window.minSize = new Vector2(560, 300);
        window.Sync();
    }

    private void OnEnable()
    {
        Sync();
    }

    private void Sync()
    {
        typeEntries = JsonDataService.Load<TypeElementEntry>(ContentEditorConfig.TypeElementFileName);
        entries = JsonDataService.Load<CreatureSpeciesEntry>(ContentEditorConfig.CreatureSpeciesFileName);
        iconCache.Clear();
        ValidateTypeReferences();
        Debug.Log($"CreatureSpeciesEditor: synced {entries.Count} entries, {typeEntries.Count} types.");
    }

    private void ValidateTypeReferences()
    {
        var validNames = new System.Collections.Generic.HashSet<string>();
        foreach (var t in typeEntries)
            validNames.Add(t.name);

        foreach (var entry in entries)
        {
            if (!string.IsNullOrEmpty(entry.type) && !validNames.Contains(entry.type))
                Debug.LogError($"CreatureSpeciesEditor: creature '{entry.name}' references unknown type '{entry.type}'.");
        }
    }

    private void Export()
    {
        JsonDataService.Save(ContentEditorConfig.CreatureSpeciesFileName, entries);
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
            entries.Add(new CreatureSpeciesEntry { stats = new CreatureStatsEntry() });
            Debug.Log("CreatureSpeciesEditor: added new entry.");
        }

        if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            if (entries.Count > 0)
            {
                entries.RemoveAt(entries.Count - 1);
                Debug.Log("CreatureSpeciesEditor: removed last entry.");
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
        GUILayout.Label("Type", GUILayout.Width(colType));
        GUILayout.Label("Stats", GUILayout.Width(colStats));
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

        GUILayout.Label((index + 1).ToString(), GUILayout.Width(colNum), GUILayout.Height(rowHeight));

        DrawIconCell(entry, index);
        DrawTextCell(entry.name, colName, "Edit Name", v => entries[index].name = v);
        DrawTypeCell(entry.type, index);
        DrawStatsCell(entry.stats, index);

        EditorGUILayout.EndHorizontal();
    }

    private void DrawIconCell(CreatureSpeciesEntry entry, int index)
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

    private void DrawTextCell(string value, float width, string popupTitle, System.Action<string> onChanged)
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

    private void DrawTypeCell(string currentType, int index)
    {
        if (GUILayout.Button(currentType, GUILayout.Width(colType), GUILayout.Height(rowHeight)))
        {
            TypeSelectorPopup.Show(
                typeEntries,
                currentType,
                v =>
                {
                    entries[index].type = v;
                    Repaint();
                },
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition)
            );
        }
    }

    private void DrawStatsCell(CreatureStatsEntry stats, int index)
    {
        var label = $"ATK:{stats.attack} HP:{stats.health} SPD:{stats.speed}";
        if (GUILayout.Button(label, GUILayout.Width(colStats), GUILayout.Height(rowHeight)))
        {
            StatsEditPopup.Show(
                stats,
                v =>
                {
                    entries[index].stats = v;
                    Repaint();
                },
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition)
            );
        }
    }
}

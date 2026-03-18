using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreatureSpeciesEditorWindow : ContentEditorWindow
{
    private static readonly float colName = 140f;
    private static readonly float colType = 100f;
    private static readonly float colStats = 160f;

    private List<CreatureSpeciesEntry> entries = new();
    private List<TypeElementEntry> typeEntries = new();

    protected override string WindowTitle => "CreatureSpecies";
    protected override string IconsFolderPath => ContentEditorConfig.SpeciesIconsFolderPath;
    protected override int EntryCount => entries.Count;

    [MenuItem("Window/Familiars/CreatureSpecies Editor")]
    public static void Open()
    {
        var window = GetWindow<CreatureSpeciesEditorWindow>("CreatureSpecies Editor");
        window.minSize = new Vector2(560, 300);
    }

    protected override void OnSync()
    {
        typeEntries = JsonDataService.Load<TypeElementEntry>(
            ContentEditorConfig.TypeElementFileName
        );
        entries = JsonDataService.Load<CreatureSpeciesEntry>(
            ContentEditorConfig.CreatureSpeciesFileName
        );
        ValidateTypeReferences();
    }

    protected override void OnExport()
    {
        JsonDataService.Save(ContentEditorConfig.CreatureSpeciesFileName, entries);
    }

    protected override void OnAddEntry()
    {
        var defaultType = typeEntries.Count > 0 ? typeEntries[0].name : "";
        entries.Add(
            new CreatureSpeciesEntry { type = defaultType, stats = new CreatureStatsEntry() }
        );
    }

    protected override void OnRemoveLastEntry()
    {
        if (entries.Count > 0)
        {
            entries.RemoveAt(entries.Count - 1);
        }
    }

    protected override void DrawColumnHeaders()
    {
        GUILayout.Label("Name", GUILayout.Width(colName));
        GUILayout.Label("Type", GUILayout.Width(colType));
        GUILayout.Label("Stats", GUILayout.Width(colStats));
    }

    protected override void DrawRow(int index)
    {
        var entry = entries[index];
        DrawIconCell(entry.icon, v => entries[index].icon = v);
        DrawTextCell(entry.name, colName, "Edit Name", v => entries[index].name = v);
        DrawTypeCell(entry.type, index);
        DrawStatsCell(entry.stats, index);
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

    private void ValidateTypeReferences()
    {
        var validNames = new HashSet<string>();
        foreach (var t in typeEntries)
            validNames.Add(t.name);

        foreach (var entry in entries)
        {
            if (!string.IsNullOrEmpty(entry.type) && !validNames.Contains(entry.type))
                Debug.LogError(
                    $"CreatureSpeciesEditor: creature '{entry.name}' references unknown type '{entry.type}'."
                );
        }
    }
}

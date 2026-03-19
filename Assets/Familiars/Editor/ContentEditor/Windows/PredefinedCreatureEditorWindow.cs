using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PredefinedCreatureEditorWindow : ContentEditorWindow
{
    private static readonly float colSpecies = 160f;
    private static readonly float colMove = 120f;

    private List<PredefinedCreatureEntry> entries = new();
    private List<CreatureSpeciesEntry> speciesEntries = new();
    private List<MoveEntry> moveEntries = new();
    private Dictionary<string, string> speciesIconByName = new();

    protected override string WindowTitle => "PredefinedCreature";
    protected override string IconsFolderPath => ContentEditorConfig.SpeciesIconsFolderPath;
    protected override bool HasIconColumn => false;
    protected override int EntryCount => entries.Count;

    [MenuItem("Window/Familiars/PredefinedCreature Editor")]
    public static void Open()
    {
        var window = GetWindow<PredefinedCreatureEditorWindow>("PredefinedCreature Editor");
        window.minSize = new Vector2(680, 300);
    }

    protected override void OnSync()
    {
        speciesEntries = JsonDataService.Load<CreatureSpeciesEntry>(
            ContentEditorConfig.CreatureSpeciesFileName
        );
        speciesIconByName = new Dictionary<string, string>();
        foreach (var s in speciesEntries)
            speciesIconByName[s.name] = s.icon ?? "";
        moveEntries = JsonDataService.Load<MoveEntry>(ContentEditorConfig.MoveFileName);
        entries = JsonDataService.Load<PredefinedCreatureEntry>(
            ContentEditorConfig.PredefinedCreatureFileName
        );
        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(entry.creatureId))
                entry.creatureId = Guid.NewGuid().ToString();
        }
        ValidateReferences();
    }

    protected override void OnExport()
    {
        JsonDataService.Save(ContentEditorConfig.PredefinedCreatureFileName, entries);
    }

    protected override void OnAddEntry()
    {
        var defaultSpecies = speciesEntries.Count > 0 ? speciesEntries[0].name : "";
        entries.Add(
            new PredefinedCreatureEntry
            {
                creatureId = Guid.NewGuid().ToString(),
                species = defaultSpecies,
                moves = new[] { "", "", "", "" },
            }
        );
    }

    protected override void OnRemoveLastEntry()
    {
        if (entries.Count > 0)
            entries.RemoveAt(entries.Count - 1);
    }

    protected override void DrawColumnHeaders()
    {
        GUILayout.Label("Species", GUILayout.Width(colSpecies));
        GUILayout.Label("Move 1", GUILayout.Width(colMove));
        GUILayout.Label("Move 2", GUILayout.Width(colMove));
        GUILayout.Label("Move 3", GUILayout.Width(colMove));
        GUILayout.Label("Move 4", GUILayout.Width(colMove));
    }

    protected override void DrawRow(int index)
    {
        var entry = entries[index];
        DrawSpeciesCell(entry.species, index);
        for (var slot = 0; slot < 4; slot++)
            DrawMoveCell(entry.moves, slot, index);
    }

    private void DrawSpeciesCell(string currentSpecies, int index)
    {
        var label = string.IsNullOrEmpty(currentSpecies) ? "(none)" : currentSpecies;
        var iconKey = GetSpeciesIconFileName(currentSpecies);
        if (
            IconKeyedOptionPicker.DrawTextWithIconButton(
                iconCache,
                label,
                iconKey,
                colSpecies,
                rowHeight
            )
        )
        {
            var names = SelectorPopupUtils.BuildNames(speciesEntries, s => s.name);
            var iconFiles = new string[speciesEntries.Count];
            for (var i = 0; i < speciesEntries.Count; i++)
                iconFiles[i] = speciesEntries[i].icon ?? "";
            IconKeyedOptionPicker.Show(
                "Select Species",
                "No species available. Sync CreatureSpecies editor first.",
                names,
                iconFiles,
                v =>
                {
                    entries[index].species = v;
                    iconCache.Clear();
                    Repaint();
                },
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition),
                IconsFolderPath
            );
        }
    }

    private string GetSpeciesIconFileName(string speciesName)
    {
        if (string.IsNullOrEmpty(speciesName))
            return "";
        return speciesIconByName.TryGetValue(speciesName, out var fileName) ? fileName : "";
    }

    private void DrawMoveCell(string[] moves, int slot, int index)
    {
        var current = moves != null && slot < moves.Length ? moves[slot] : "";
        var label = string.IsNullOrEmpty(current) ? "(none)" : current;
        if (GUILayout.Button(label, GUILayout.Width(colMove), GUILayout.Height(rowHeight)))
        {
            SelectorPopup.Show(
                "Select Move",
                "No moves available. Sync Move editor first.",
                SelectorPopupUtils.BuildNames(moveEntries, m => m.name),
                v =>
                {
                    entries[index].moves[slot] = v;
                    Repaint();
                },
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition),
                allowNone: true
            );
        }
    }

    private void ValidateReferences()
    {
        var validSpecies = new HashSet<string>();
        foreach (var s in speciesEntries)
            validSpecies.Add(s.name);

        var validMoves = new HashSet<string>();
        foreach (var m in moveEntries)
            validMoves.Add(m.name);

        foreach (var entry in entries)
        {
            if (!string.IsNullOrEmpty(entry.species) && !validSpecies.Contains(entry.species))
                Debug.LogError(
                    $"PredefinedCreatureEditor: creature '{entry.creatureId}' references unknown species '{entry.species}'."
                );

            if (entry.moves != null)
            {
                foreach (var move in entry.moves)
                {
                    if (!string.IsNullOrEmpty(move) && !validMoves.Contains(move))
                        Debug.LogError(
                            $"PredefinedCreatureEditor: creature '{entry.creatureId}' references unknown move '{move}'."
                        );
                }
            }
        }
    }
}

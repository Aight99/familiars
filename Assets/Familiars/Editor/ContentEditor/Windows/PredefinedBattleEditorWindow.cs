using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PredefinedBattleEditorWindow : ContentEditorWindow
{
    private static readonly float colSpecies = 160f;
    private static readonly float colMove = 120f;

    private List<PredefinedBattleEntry> battles = new();
    private List<bool> battleFoldouts = new();
    private List<CreatureSpeciesEntry> speciesEntries = new();
    private List<MoveEntry> moveEntries = new();
    private Dictionary<string, string> speciesIconByName = new();

    protected override string WindowTitle => "PredefinedBattle";
    protected override string IconsFolderPath => ContentEditorConfig.SpeciesIconsFolderPath;
    protected override bool HasIconColumn => false;
    protected override int EntryCount => battles.Count;

    [MenuItem("Window/Familiars/Predefined Battle Editor")]
    public static void Open()
    {
        var window = GetWindow<PredefinedBattleEditorWindow>("Predefined Battle Editor");
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

        battles = JsonDataService.Load<PredefinedBattleEntry>(
            ContentEditorConfig.PredefinedBattleFileName
        );
        var migrated = false;
        if (battles.Count == 0)
        {
            var legacy = JsonDataService.Load<PredefinedCreatureEntry>(
                ContentEditorConfig.LegacyPredefinedCreatureFileName
            );
            if (legacy.Count > 0)
            {
                battles.Add(
                    new PredefinedBattleEntry
                    {
                        name = "Battle",
                        creatures = new List<PredefinedCreatureEntry>(legacy),
                    }
                );
                migrated = true;
            }
        }

        foreach (var battle in battles)
        {
            if (battle.creatures == null)
                battle.creatures = new List<PredefinedCreatureEntry>();
        }

        NormalizeAllCreatureIds();
        GroupedEditorSection.EnsureFoldoutCount(battleFoldouts, battles.Count);
        ValidateReferences();

        if (migrated)
            JsonDataService.Save(ContentEditorConfig.PredefinedBattleFileName, battles);
    }

    protected override void OnExport()
    {
        JsonDataService.Save(ContentEditorConfig.PredefinedBattleFileName, battles);
    }

    protected override void OnAddEntry()
    {
        battles.Add(
            new PredefinedBattleEntry
            {
                name = "",
                creatures = new List<PredefinedCreatureEntry> { CreateDefaultCreatureEntry() },
            }
        );
        battleFoldouts.Add(true);
    }

    protected override void OnRemoveLastEntry()
    {
        if (battles.Count == 0)
            return;
        battles.RemoveAt(battles.Count - 1);
        GroupedEditorSection.EnsureFoldoutCount(battleFoldouts, battles.Count);
    }

    protected override void DrawHeaderRow() { }

    protected override void DrawTableBody()
    {
        for (var g = 0; g < battles.Count; g++)
        {
            var groupIndex = g;
            var battle = battles[groupIndex];
            battleFoldouts[groupIndex] = GroupedEditorSection.DrawHeader(
                battleFoldouts[groupIndex],
                battle.name,
                "Battle name",
                v =>
                {
                    battle.name = v;
                    Repaint();
                },
                () =>
                {
                    battle.creatures.Add(CreateDefaultCreatureEntry());
                    Repaint();
                },
                () =>
                {
                    if (battle.creatures.Count > 0)
                    {
                        battle.creatures.RemoveAt(battle.creatures.Count - 1);
                        Repaint();
                    }
                }
            );

            if (!battleFoldouts[groupIndex])
                continue;

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("#", GUILayout.Width(colNum));
            DrawColumnHeaders();
            EditorGUILayout.EndHorizontal();

            for (var r = 0; r < battle.creatures.Count; r++)
            {
                var creatureIndex = r;
                EditorGUILayout.BeginHorizontal(GUILayout.Height(rowHeight));
                GUILayout.Label(
                    (creatureIndex + 1).ToString(),
                    GUILayout.Width(colNum),
                    GUILayout.Height(rowHeight)
                );
                DrawCreatureRow(groupIndex, creatureIndex);
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    protected override void DrawColumnHeaders()
    {
        GUILayout.Label("Species", GUILayout.Width(colSpecies));
        GUILayout.Label("Move 1", GUILayout.Width(colMove));
        GUILayout.Label("Move 2", GUILayout.Width(colMove));
        GUILayout.Label("Move 3", GUILayout.Width(colMove));
        GUILayout.Label("Move 4", GUILayout.Width(colMove));
    }

    protected override void DrawRow(int index) { }

    private PredefinedCreatureEntry CreateDefaultCreatureEntry()
    {
        var defaultSpecies = speciesEntries.Count > 0 ? speciesEntries[0].name : "";
        return new PredefinedCreatureEntry
        {
            creatureId = Guid.NewGuid().ToString(),
            species = defaultSpecies,
            moves = new[] { "", "", "", "" },
        };
    }

    private void NormalizeAllCreatureIds()
    {
        foreach (var battle in battles)
        {
            if (battle.creatures == null)
                continue;
            foreach (var entry in battle.creatures)
            {
                if (string.IsNullOrEmpty(entry.creatureId))
                    entry.creatureId = Guid.NewGuid().ToString();
            }
        }
    }

    private void DrawCreatureRow(int battleIndex, int creatureIndex)
    {
        var entry = battles[battleIndex].creatures[creatureIndex];
        DrawSpeciesCell(entry.species, battleIndex, creatureIndex);
        for (var slot = 0; slot < 4; slot++)
            DrawMoveCell(entry.moves, slot, battleIndex, creatureIndex);
    }

    private void DrawSpeciesCell(string currentSpecies, int battleIndex, int creatureIndex)
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
                    battles[battleIndex].creatures[creatureIndex].species = v;
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

    private void DrawMoveCell(string[] moves, int slot, int battleIndex, int creatureIndex)
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
                    battles[battleIndex].creatures[creatureIndex].moves[slot] = v;
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

        foreach (var battle in battles)
        {
            if (battle.creatures == null)
                continue;
            var battleLabel = string.IsNullOrEmpty(battle.name) ? "(unnamed)" : battle.name;
            foreach (var entry in battle.creatures)
            {
                if (!string.IsNullOrEmpty(entry.species) && !validSpecies.Contains(entry.species))
                    Debug.LogError(
                        $"PredefinedBattleEditor: battle '{battleLabel}', creature '{entry.creatureId}' references unknown species '{entry.species}'."
                    );

                if (entry.moves != null)
                {
                    foreach (var move in entry.moves)
                    {
                        if (!string.IsNullOrEmpty(move) && !validMoves.Contains(move))
                            Debug.LogError(
                                $"PredefinedBattleEditor: battle '{battleLabel}', creature '{entry.creatureId}' references unknown move '{move}'."
                            );
                    }
                }
            }
        }
    }
}

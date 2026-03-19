using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveEditorWindow : ContentEditorWindow
{
    private static readonly float colName = 140f;
    private static readonly float colPower = 60f;
    private static readonly float colType = 50f;
    private static readonly float colAppType = 50f;

    private List<MoveEntry> entries = new();
    private List<TypeElementEntry> typeEntries = new();
    private Dictionary<string, string> typeIconByName = new();
    private IconCache appTypeIconCache;

    protected override string WindowTitle => "Move";
    protected override string IconsFolderPath => ContentEditorConfig.TypeIconsFolderPath;
    protected override bool HasIconColumn => false;
    protected override int EntryCount => entries.Count;

    private IconCache AppTypeIconCache =>
        appTypeIconCache ??= new IconCache(ContentEditorConfig.MoveApplicationTypeIconsFolderPath);

    [MenuItem("Window/Familiars/Move Editor")]
    public static void Open()
    {
        var window = GetWindow<MoveEditorWindow>("Move Editor");
        window.minSize = new Vector2(460, 300);
    }

    protected override void OnSync()
    {
        typeEntries = JsonDataService.Load<TypeElementEntry>(
            ContentEditorConfig.TypeElementFileName
        );
        typeIconByName = new Dictionary<string, string>();
        foreach (var t in typeEntries)
            typeIconByName[t.name] = t.icon ?? "";
        entries = JsonDataService.Load<MoveEntry>(ContentEditorConfig.MoveFileName);
        AppTypeIconCache.Clear();
        ValidateTypeReferences();
    }

    protected override void OnExport()
    {
        JsonDataService.Save(ContentEditorConfig.MoveFileName, entries);
    }

    protected override void OnAddEntry()
    {
        var defaultType = typeEntries.Count > 0 ? typeEntries[0].name : "";
        entries.Add(new MoveEntry { type = defaultType, applicationType = "Physical" });
    }

    protected override void OnRemoveLastEntry()
    {
        if (entries.Count > 0)
            entries.RemoveAt(entries.Count - 1);
    }

    protected override void DrawColumnHeaders()
    {
        GUILayout.Label("Name", GUILayout.Width(colName));
        GUILayout.Label("Power", GUILayout.Width(colPower));
        GUILayout.Label("Type", GUILayout.Width(colType));
        GUILayout.Label("Applic", GUILayout.Width(colAppType));
    }

    protected override void DrawRow(int index)
    {
        var entry = entries[index];
        DrawTextCell(entry.name, colName, "Edit Name", v => entries[index].name = v);
        DrawPowerCell(entry.power, index);
        DrawTypeCell(entry.type, index);
        DrawAppTypeCell(entry.applicationType, index);
    }

    private void DrawPowerCell(int power, int index)
    {
        if (
            GUILayout.Button(
                power.ToString(),
                GUILayout.Width(colPower),
                GUILayout.Height(rowHeight)
            )
        )
        {
            IntEditPopup.Show(
                "Edit Power",
                power,
                v =>
                {
                    entries[index].power = v;
                    Repaint();
                },
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition)
            );
        }
    }

    private void DrawTypeCell(string currentType, int index)
    {
        var iconKey = GetTypeIconFileName(currentType);
        if (
            IconKeyedOptionPicker.DrawTextWithIconButton(
                iconCache,
                currentType,
                iconKey,
                colType,
                rowHeight,
                showText: false
            )
        )
        {
            var names = SelectorPopupUtils.BuildNames(typeEntries, t => t.name);
            var iconFiles = new string[typeEntries.Count];
            for (var i = 0; i < typeEntries.Count; i++)
                iconFiles[i] = typeEntries[i].icon ?? "";
            IconKeyedOptionPicker.Show(
                "Select Type",
                "No types available. Sync TypeElement editor first.",
                names,
                iconFiles,
                v =>
                {
                    entries[index].type = v;
                    iconCache.Clear();
                    Repaint();
                },
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition),
                ContentEditorConfig.TypeIconsFolderPath
            );
        }
    }

    private string GetTypeIconFileName(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
            return "";
        return typeIconByName.TryGetValue(typeName, out var fileName) ? fileName : "";
    }

    private void DrawAppTypeCell(string currentAppType, int index)
    {
        if (
            IconKeyedOptionPicker.DrawIconOnlyButton(
                AppTypeIconCache,
                currentAppType,
                colAppType,
                rowHeight
            )
        )
        {
            IconKeyedOptionPicker.ShowWhenIconFileNameMatchesValue(
                "Select Application Type",
                "No application types available.",
                new[] { "Physical", "Ranged", "Status" },
                v =>
                {
                    entries[index].applicationType = v;
                    AppTypeIconCache.Clear();
                    Repaint();
                },
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition),
                ContentEditorConfig.MoveApplicationTypeIconsFolderPath
            );
        }
    }

    private void ValidateTypeReferences()
    {
        var validNames = new System.Collections.Generic.HashSet<string>();
        foreach (var t in typeEntries)
            validNames.Add(t.name);

        foreach (var entry in entries)
        {
            if (!string.IsNullOrEmpty(entry.type) && !validNames.Contains(entry.type))
                Debug.LogError(
                    $"MoveEditor: move '{entry.name}' references unknown type '{entry.type}'."
                );
        }
    }
}

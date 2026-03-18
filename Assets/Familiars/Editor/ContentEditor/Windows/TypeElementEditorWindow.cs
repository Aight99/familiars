using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TypeElementEditorWindow : ContentEditorWindow
{
    private static readonly float colName = 120f;
    private static readonly float colEffective = 160f;
    private static readonly float colIneffective = 160f;

    private List<TypeElementEntry> entries = new();

    protected override string WindowTitle => "TypeElement";
    protected override string IconsFolderPath => ContentEditorConfig.TypeIconsFolderPath;
    protected override int EntryCount => entries.Count;

    [MenuItem("Window/Familiars/TypeElement Editor")]
    public static void Open()
    {
        var window = GetWindow<TypeElementEditorWindow>("TypeElement Editor");
        window.minSize = new Vector2(600, 300);
    }

    protected override void OnSync()
    {
        entries = JsonDataService.Load<TypeElementEntry>(ContentEditorConfig.TypeElementFileName);
    }

    protected override void OnExport()
    {
        JsonDataService.Save(ContentEditorConfig.TypeElementFileName, entries);
    }

    protected override void OnAddEntry()
    {
        entries.Add(new TypeElementEntry());
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
        GUILayout.Label("Effective Against", GUILayout.Width(colEffective));
        GUILayout.Label("Ineffective Against", GUILayout.Width(colIneffective));
    }

    protected override void DrawRow(int index)
    {
        var entry = entries[index];
        DrawIconCell(entry.icon, v => entries[index].icon = v);
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

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpeciesSelectorPopup : EditorWindow
{
    private string[] speciesNames = Array.Empty<string>();
    private int selectedIndex;
    private Action<string> onChange;

    public static void Show(
        List<CreatureSpeciesEntry> species,
        string currentValue,
        Action<string> onChange,
        Vector2 screenPosition
    )
    {
        var window = CreateInstance<SpeciesSelectorPopup>();
        window.titleContent = new GUIContent("Select Species");
        window.speciesNames = BuildNames(species);
        window.selectedIndex = FindIndex(window.speciesNames, currentValue);
        window.onChange = onChange;
        window.ShowUtility();
        window.position = new Rect(screenPosition.x, screenPosition.y, 220, 50);
    }

    private void OnGUI()
    {
        if (speciesNames.Length == 0)
        {
            EditorGUILayout.LabelField("No species available. Sync CreatureSpecies editor first.");
            return;
        }

        var newIndex = EditorGUILayout.Popup("Species", selectedIndex, speciesNames);
        if (newIndex != selectedIndex)
        {
            selectedIndex = newIndex;
            onChange?.Invoke(speciesNames[selectedIndex]);
        }
    }

    private static string[] BuildNames(List<CreatureSpeciesEntry> species)
    {
        if (species == null || species.Count == 0)
            return Array.Empty<string>();

        var names = new string[species.Count];
        for (var i = 0; i < species.Count; i++)
            names[i] = species[i].name;
        return names;
    }

    private static int FindIndex(string[] names, string value)
    {
        for (var i = 0; i < names.Length; i++)
        {
            if (names[i] == value)
                return i;
        }
        return 0;
    }
}

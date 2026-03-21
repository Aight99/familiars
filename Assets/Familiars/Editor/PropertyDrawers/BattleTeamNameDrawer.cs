using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BattleTeamNameAttribute))]
public sealed class BattleTeamNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        if (property.hasMultipleDifferentValues)
        {
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndProperty();
            return;
        }

        var teams = JsonDataService.Load<BattleTeamEntry>(ContentEditorConfig.BattleTeamsFileName);
        var teamNames = new List<string>();
        foreach (var entry in teams)
        {
            if (string.IsNullOrEmpty(entry.name))
                continue;
            teamNames.Add(entry.name);
        }

        if (teamNames.Count == 0)
        {
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndProperty();
            return;
        }

        var current = property.stringValue;
        var displayOptions = new List<string>();
        var mappedValues = new List<string>();
        displayOptions.Add("(None)");
        mappedValues.Add("");

        if (!string.IsNullOrEmpty(current) && !teamNames.Contains(current))
        {
            displayOptions.Add("Unknown: " + current);
            mappedValues.Add(current);
        }

        foreach (var name in teamNames)
        {
            displayOptions.Add(name);
            mappedValues.Add(name);
        }

        var index = mappedValues.IndexOf(current);
        if (index < 0)
            index = 0;

        index = EditorGUI.Popup(position, label.text, index, displayOptions.ToArray());
        property.stringValue = mappedValues[index];

        EditorGUI.EndProperty();
    }
}

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class GroupedEditorSection
{
    private const float MinimumHeaderControlHeight = 20f;
    private const float FoldoutWidth = 16f;
    private const float FoldoutToNameGap = 4f;
    private const float NameToChildButtonsGap = 6f;
    private const float ChildButtonsGap = 0f;
    private const float NameButtonWidth = 200f;
    private const string UnnamedDisplayLabel = "(unnamed)";

    private static GUIStyle headerRowButtonStyle;

    public static bool DrawHeader(
        bool expanded,
        string name,
        string namePopupTitle,
        Action<string> setName,
        Action addChild,
        Action removeLastChild
    )
    {
        var controlHeight = Mathf.Max(
            EditorGUIUtility.singleLineHeight,
            MinimumHeaderControlHeight
        );
        var style = HeaderRowButtonStyle;

        EditorGUILayout.BeginHorizontal(GUILayout.Height(controlHeight));

        var foldoutRect = GUILayoutUtility.GetRect(
            FoldoutWidth,
            controlHeight,
            GUILayout.Width(FoldoutWidth)
        );
        expanded = EditorGUI.Foldout(foldoutRect, expanded, string.Empty);

        GUILayout.Space(FoldoutToNameGap);

        var displayName = string.IsNullOrEmpty(name) ? UnnamedDisplayLabel : name;
        if (
            GUILayout.Button(
                displayName,
                style,
                GUILayout.Width(NameButtonWidth),
                GUILayout.Height(controlHeight)
            )
        )
        {
            TextEditPopup.Show(
                namePopupTitle,
                name,
                v =>
                {
                    setName(v);
                },
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition)
            );
        }

        GUILayout.Space(NameToChildButtonsGap);

        if (
            GUILayout.Button(
                "+",
                style,
                GUILayout.Width(controlHeight),
                GUILayout.Height(controlHeight)
            )
        )
            addChild();

        GUILayout.Space(ChildButtonsGap);

        if (
            GUILayout.Button(
                "-",
                style,
                GUILayout.Width(controlHeight),
                GUILayout.Height(controlHeight)
            )
        )
            removeLastChild();

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();
        return expanded;
    }

    private static GUIStyle HeaderRowButtonStyle
    {
        get
        {
            if (headerRowButtonStyle == null)
            {
                headerRowButtonStyle = new GUIStyle(EditorStyles.toolbarButton)
                {
                    fixedHeight = 0,
                    stretchHeight = false,
                    stretchWidth = false,
                    padding = new RectOffset(6, 6, 1, 1),
                    alignment = TextAnchor.MiddleCenter,
                };
            }
            return headerRowButtonStyle;
        }
    }

    public static void EnsureFoldoutCount(
        List<bool> expanded,
        int groupCount,
        bool defaultExpanded = true
    )
    {
        while (expanded.Count < groupCount)
            expanded.Add(defaultExpanded);
        while (expanded.Count > groupCount && expanded.Count > 0)
            expanded.RemoveAt(expanded.Count - 1);
    }
}

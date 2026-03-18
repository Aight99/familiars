using System;
using UnityEditor;
using UnityEngine;

public class StatsEditPopup : EditorWindow
{
    private CreatureStatsEntry stats;
    private Action<CreatureStatsEntry> onChange;

    public static void Show(
        CreatureStatsEntry currentStats,
        Action<CreatureStatsEntry> onChange,
        Vector2 screenPosition
    )
    {
        var window = CreateInstance<StatsEditPopup>();
        window.titleContent = new GUIContent("Edit Stats");
        window.stats = new CreatureStatsEntry
        {
            attack = currentStats.attack,
            health = currentStats.health,
            speed = currentStats.speed,
        };
        window.onChange = onChange;
        window.ShowUtility();
        window.position = new Rect(screenPosition.x, screenPosition.y, 220, 90);
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        stats.attack = EditorGUILayout.IntField("Attack", stats.attack);
        stats.health = EditorGUILayout.IntField("Health", stats.health);
        stats.speed = EditorGUILayout.IntField("Speed", stats.speed);

        if (EditorGUI.EndChangeCheck())
        {
            onChange?.Invoke(
                new CreatureStatsEntry
                {
                    attack = stats.attack,
                    health = stats.health,
                    speed = stats.speed,
                }
            );
        }
    }
}

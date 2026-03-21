using System;
using System.Collections.Generic;
using UnityEngine;

public class CreaturePrefabRegistry : MonoBehaviour
{
    private const string CreatureModelsResourcePrefix = "CreatureModels/";

    [SerializeField]
    private GameObject battleTemplate;

    [SerializeField]
    private GameObject overworldTemplate;

    private readonly Dictionary<string, GameObject> basePrefabs = new Dictionary<
        string,
        GameObject
    >(StringComparer.Ordinal);

    public void Initialize(GameDataService gameDataService)
    {
        basePrefabs.Clear();
        foreach (var speciesName in gameDataService.AllSpeciesNames)
        {
            var path = CreatureModelsResourcePrefix + speciesName;
            var prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
                Debug.LogError(
                    $"CreaturePrefabRegistry: no prefab at Resources/{path} for species '{speciesName}'."
                );
            else
                basePrefabs[speciesName] = prefab;
        }
    }

    public GameObject SpawnForBattle(string speciesName, Transform parent)
    {
        return SpawnWithTemplate(speciesName, parent, battleTemplate, "battle");
    }

    public GameObject SpawnForOverworld(string speciesName, Transform parent)
    {
        return SpawnWithTemplate(speciesName, parent, overworldTemplate, "overworld");
    }

    private GameObject SpawnWithTemplate(
        string speciesName,
        Transform parent,
        GameObject template,
        string contextLabel
    )
    {
        if (string.IsNullOrEmpty(speciesName))
            return null;

        if (!basePrefabs.TryGetValue(speciesName, out var basePrefab) || basePrefab == null)
        {
            Debug.LogError(
                $"CreaturePrefabRegistry: missing base prefab for species '{speciesName}' ({contextLabel})."
            );
            return null;
        }

        var instance = Instantiate(basePrefab, parent);
        if (template == null)
            Debug.LogError($"CreaturePrefabRegistry: {contextLabel} template is not assigned.");
        else
            ApplyTemplate(template, instance);
        return instance;
    }

    private static void ApplyTemplate(GameObject templatePrefab, GameObject instance)
    {
        if (templatePrefab == null || instance == null)
            return;

        foreach (var source in templatePrefab.GetComponents<Component>())
        {
            if (source == null || source is Transform)
                continue;

            var type = source.GetType();
            if (instance.GetComponent(type) != null)
                continue;

            var added = instance.AddComponent(type);
            if (added == null)
                continue;

            var json = JsonUtility.ToJson(source);
            JsonUtility.FromJsonOverwrite(json, added);
        }
    }
}

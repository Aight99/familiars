using System;
using UnityEngine;

[Serializable]
public struct CreaturePrefabMapping
{
    [SerializeField]
    private string speciesName;

    [SerializeField]
    private GameObject prefab;

    public string SpeciesName => speciesName;
    public GameObject Prefab => prefab;
}

public class CreaturePrefabRegistry : MonoBehaviour
{
    [SerializeField]
    private CreaturePrefabMapping[] mappings = Array.Empty<CreaturePrefabMapping>();

    public GameObject GetPrefab(string speciesName)
    {
        if (string.IsNullOrEmpty(speciesName))
            return null;

        foreach (var mapping in mappings)
        {
            if (mapping.SpeciesName == speciesName)
                return mapping.Prefab;
        }

        return null;
    }
}

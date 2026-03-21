using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PredefinedSpawn
{
    [SerializeField]
    private string speciesName;

    [SerializeField]
    [BattleTeamName]
    private string battleTeamName;

    [SerializeField]
    private Transform spawnPoint;

    public string SpeciesName => speciesName;

    public string BattleTeamName => battleTeamName;

    public Transform SpawnPoint => spawnPoint;
}

public class CreatureSpawner : MonoBehaviour
{
    [SerializeField]
    private PredefinedSpawn[] spawns;

    [SerializeField]
    private OverworldManager overworldManager;

    [SerializeField]
    private CreaturePrefabRegistry creaturePrefabRegistry;

    private readonly Dictionary<string, OverworldCreature> creatures = new();

    private void Start()
    {
        if (overworldManager == null)
        {
            Debug.LogError("CreatureSpawner: OverworldManager is not assigned.");
            return;
        }

        foreach (var spawn in spawns)
        {
            var point = spawn.SpawnPoint;
            if (point == null)
            {
                Debug.LogError("CreatureSpawner: spawn point is null.");
                continue;
            }

            if (creaturePrefabRegistry == null)
            {
                Debug.LogError("CreatureSpawner: CreaturePrefabRegistry is not assigned.");
                continue;
            }

            var instance = creaturePrefabRegistry.SpawnForOverworld(spawn.SpeciesName, point);
            if (instance == null)
                continue;

            instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            var creature = instance.GetComponent<OverworldCreature>();
            if (creature == null)
            {
                Debug.LogError(
                    $"CreatureSpawner: OverworldCreature missing for species '{spawn.SpeciesName}'."
                );
                Destroy(instance);
                continue;
            }

            creature.Initialize(spawn.BattleTeamName, overworldManager.OnCreatureEncountered);
            creatures[creature.TeamName] = creature;
        }
    }

    public void DestroyCreature(string teamName)
    {
        if (creatures.TryGetValue(teamName, out var creature))
        {
            creatures.Remove(teamName);
            Destroy(creature.gameObject);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PredefinedSpawn
{
    public OverworldCreature creature;
    public Transform transition;
}

public class CreatureSpawner : MonoBehaviour
{
    [SerializeField]
    private PredefinedSpawn[] spawns;

    [SerializeField]
    private OverworldManager overworldManager;

    private readonly Dictionary<CreatureId, OverworldCreature> creatures = new();

    private void Awake()
    {
        foreach (var spawn in spawns)
        {
            var creature = Instantiate(
                spawn.creature,
                spawn.transition.position,
                spawn.transition.rotation
            );
            creature.Initialize(overworldManager.OnCreatureEncountered);
            creatures[creature.Id] = creature;
        }
    }

    public void DestroyCreature(CreatureId id)
    {
        if (creatures.TryGetValue(id, out var creature))
        {
            creatures.Remove(id);
            Destroy(creature.gameObject);
        }
    }
}

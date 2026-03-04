using System;
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
        }
    }
}

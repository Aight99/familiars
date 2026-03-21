using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField]
    private Transform playerCreaturePosition;

    [SerializeField]
    private Transform rivalCreaturePosition;

    private CreaturePrefabRegistry prefabRegistry;

    public void Initialize(CreaturePrefabRegistry registry)
    {
        prefabRegistry = registry;
    }

    public (CreatureView player, CreatureView rival) PlaceCreatures(
        Creature playerCreature,
        Creature rivalCreature
    )
    {
        var playerView = SpawnCreatureView(playerCreature, playerCreaturePosition);
        var rivalView = SpawnCreatureView(rivalCreature, rivalCreaturePosition);
        return (playerView, rivalView);
    }

    private CreatureView SpawnCreatureView(Creature creature, Transform parent)
    {
        GameObject instance = null;
        if (prefabRegistry != null)
            instance = prefabRegistry.SpawnForBattle(creature.SpeciesName, parent);

        if (instance == null)
        {
            Debug.LogError($"Field: missing prefab for species '{creature.SpeciesName}'.");
            instance = new GameObject(creature.SpeciesName);
            instance.transform.SetParent(parent, false);
        }

        var view = instance.GetComponent<CreatureView>();
        if (view == null)
        {
            Debug.LogError(
                $"Field: CreatureView missing after spawn for species '{creature.SpeciesName}'."
            );
            view = instance.AddComponent<CreatureView>();
        }

        view.Init(creature);
        return view;
    }
}

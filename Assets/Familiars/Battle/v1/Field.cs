using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField]
    private Transform playerCreaturePosition;

    [SerializeField]
    private Transform rivalCreaturePosition;

    public (CreatureView player, CreatureView rival) PlaceCreatures(
        Creature playerCreature,
        Creature rivalCreature
    )
    {
        var playerView = SpawnCreatureView(playerCreature, playerCreaturePosition);
        var rivalView = SpawnCreatureView(rivalCreature, rivalCreaturePosition);
        return (playerView, rivalView);
    }

    private static CreatureView SpawnCreatureView(Creature creature, Transform position)
    {
        var go = Instantiate(creature.Kind.Model, position);
        var view = go.GetComponent<CreatureView>() ?? go.AddComponent<CreatureView>();
        view.Init(creature);
        return view;
    }
}

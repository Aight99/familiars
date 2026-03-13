using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PredefinedCreature", menuName = "Familiars/Predefined Creature")]
public class PredefinedCreature : ScriptableObject
{
    [SerializeField]
    private CreatureId creatureId;

    [SerializeField]
    private CreatureKind kind;

    [SerializeField]
    private List<Move> moves = new() { Move.None, Move.None, Move.None, Move.None };

    public CreatureKind Kind => kind;
    public IReadOnlyList<Move> Moves => moves;
    public CreatureId CreatureId => creatureId;

    private void OnEnable()
    {
        if (creatureId.IsEmpty)
        {
            creatureId = CreatureId.Generate();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    public Creature MakeCreature()
    {
        return new Creature(
            creatureId,
            kind,
            kind.Type,
            kind.Health,
            kind.Attack,
            kind.Speed,
            moves
        );
    }

    private void OnValidate()
    {
        if (moves == null)
        {
            Debug.LogError("PredefinedCreature.moves == null");
        }

        if (moves.Count != 4)
        {
            Debug.LogError("PredefinedCreature.moves неправильного размера");
        }
    }
}

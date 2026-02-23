using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Creature", menuName = "Familiars/Creature")]
public class Creature : ScriptableObject
{
    [SerializeField]
    private CreatureKind kind;

    [SerializeField]
    private List<Move> moves = new() { Move.None, Move.None, Move.None, Move.None };

    public CreatureKind Kind => kind;
    public IReadOnlyList<Move> Moves => moves;

    private void OnValidate()
    {
        if (moves == null)
        {
            Debug.LogError("Creature.moves == null");
        }

        if (moves.Count != 4)
        {
            Debug.LogError("Creature.moves неправильного размера");
        }
    }
}

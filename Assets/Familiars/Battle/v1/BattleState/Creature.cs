using System.Collections.Generic;

public class Creature
{
    public CreatureId Id { get; }
    public CreatureKind Kind { get; }
    public TypeElement Type { get; private set; }
    public int Health { get; private set; }
    public int Attack { get; private set; }
    public int Speed { get; private set; }
    public IReadOnlyList<Move> Moves { get; private set; }

    public Creature(
        CreatureKind kind,
        TypeElement type,
        int health,
        int attack,
        int speed,
        IReadOnlyList<Move> moves
    )
    {
        Id = CreatureId.Generate();
        Kind = kind;
        Type = type;
        Health = health;
        Attack = attack;
        Speed = speed;
        Moves = moves;
    }
}

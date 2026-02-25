using System.Collections.Generic;

public class Creature
{
    public string KindName { get; }
    public TypeElement Type { get; }
    public int Health { get; }
    public int Attack { get; }
    public int Speed { get; }
    public IReadOnlyList<Move> Moves { get; }

    public Creature(
        string kindName,
        TypeElement type,
        int health,
        int attack,
        int speed,
        IReadOnlyList<Move> moves
    )
    {
        KindName = kindName;
        Type = type;
        Health = health;
        Attack = attack;
        Speed = speed;
        Moves = moves;
    }
}

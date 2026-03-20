using System.Collections.Generic;

public readonly struct BattleTeamCreature
{
    public string CreatureId { get; }
    public CreatureSpecies Species { get; }
    public IReadOnlyList<Move> Moves { get; }

    public BattleTeamCreature(string creatureId, CreatureSpecies species, IReadOnlyList<Move> moves)
    {
        CreatureId = creatureId;
        Species = species;
        Moves = moves;
    }
}

public class BattleTeam
{
    public string Name { get; }
    public IReadOnlyList<BattleTeamCreature> Creatures { get; }

    public BattleTeam(string name, IReadOnlyList<BattleTeamCreature> creatures)
    {
        Name = name;
        Creatures = creatures;
    }
}

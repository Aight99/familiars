using System.Collections.Generic;

public class BattleState
{
    private readonly Dictionary<CreatureId, Creature> creatures = new();

    public CreatureId PlayerCreatureId { get; }
    public CreatureId RivalCreatureId { get; }
    public int TurnCount { get; private set; }

    private BattleState(Creature playerCreature, Creature rivalCreature)
    {
        creatures[playerCreature.Id] = playerCreature;
        creatures[rivalCreature.Id] = rivalCreature;
        PlayerCreatureId = playerCreature.Id;
        RivalCreatureId = rivalCreature.Id;
        TurnCount = 1;
    }

    public static BattleState FromLevelConfig(LevelConfig levelConfig)
    {
        return new BattleState(
            levelConfig.PlayerCreature.MakeCreature(),
            levelConfig.RivalCreature.MakeCreature()
        );
    }

    public Creature GetCreature(CreatureId id) => creatures[id];

    public void IncrementTurn()
    {
        TurnCount++;
    }
}

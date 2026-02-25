public class BattleState
{
    public Creature PlayerCreature { get; }
    public Creature RivalCreature { get; }
    public int TurnCount { get; private set; }

    private BattleState(Creature playerCreature, Creature rivalCreature)
    {
        PlayerCreature = playerCreature;
        RivalCreature = rivalCreature;
        TurnCount = 1;
    }

    public static BattleState FromLevelConfig(LevelConfig levelConfig)
    {
        return new BattleState(
            levelConfig.PlayerCreature.MakeCreature(),
            levelConfig.RivalCreature.MakeCreature()
        );
    }

    public void IncrementTurn()
    {
        TurnCount++;
    }
}

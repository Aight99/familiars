public class BattleConfig
{
    public BattleTeam PlayerTeam { get; }
    public BattleTeam RivalTeam { get; }

    public BattleConfig(BattleTeam playerTeam, BattleTeam rivalTeam)
    {
        PlayerTeam = playerTeam;
        RivalTeam = rivalTeam;
    }
}

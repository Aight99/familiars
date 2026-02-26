readonly struct CreatureMoveCommand : ICommand
{
    private readonly CreatureId userId;
    private readonly CreatureId targetId;
    private readonly Move move;

    public CreatureMoveCommand(CreatureId userId, CreatureId targetId, Move move)
    {
        this.userId = userId;
        this.targetId = targetId;
        this.move = move;
    }

    public readonly void Execute(BattleState state) { }

    public readonly int GetPriority(BattleState state)
    {
        return -state.GetCreature(userId).Speed;
    }
}

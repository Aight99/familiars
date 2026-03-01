readonly struct DummyCommand : ICommand
{
    private readonly string name;

    public DummyCommand(string name)
    {
        this.name = name;
    }

    public readonly void Execute(BattleState state)
    {
        DebugHelper.Log(DebugHelper.MessageType.Yippee, $"{name} executed");
    }

    public readonly int GetPriority(BattleState state)
    {
        return 0;
    }

    public readonly MoveAnimationData? GetAnimationData(BattleState state) => null;
}

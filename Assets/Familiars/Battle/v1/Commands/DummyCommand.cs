readonly struct DummyCommand : ICommand
{
    private readonly string name;

    public DummyCommand(string name)
    {
        this.name = name;
    }

    public readonly void Execute()
    {
        DebugHelper.Log(DebugHelper.MessageType.Yippee, $"{name} executed");
    }

    public readonly int GetPriority()
    {
        return 0;
    }
}

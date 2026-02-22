struct DummyCommand : ICommand
{
    public readonly void Execute()
    {
        DebugHelper.Log(DebugHelper.MessageType.Yippee, "DummyCommand executed");
    }

    public readonly int GetPriority()
    {
        return 0;
    }
}

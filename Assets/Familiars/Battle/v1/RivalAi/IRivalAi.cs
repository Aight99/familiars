using UnityEngine;

public interface IRivalAi
{
    ICommand GetCommand();
}

public class YoungsterJoey : IRivalAi
{
    private static readonly string[] commandNames = { "Tackle", "Bite", "Super Fang", "Tail Whip" };

    public ICommand GetCommand()
    {
        var name = commandNames[Random.Range(0, commandNames.Length)];
        return new DummyCommand(name);
    }
}

public class BugTrainerBrandon : IRivalAi
{
    private static readonly string[] commandNames =
    {
        "Bug Buzz",
        "X-Scissor",
        "Iron Defense",
        "Giga Drain",
    };

    public ICommand GetCommand()
    {
        var name = commandNames[Random.Range(0, commandNames.Length)];
        return new DummyCommand(name);
    }
}

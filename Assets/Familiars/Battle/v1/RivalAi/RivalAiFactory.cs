using UnityEngine;

public static class RivalAiFactory
{
    public static IRivalAi Create()
    {
        return CreateRandomRival();
    }

    private static IRivalAi CreateRandomRival()
    {
        return Random.Range(0, 2) switch
        {
            0 => new YoungsterJoey(),
            1 => new BugTrainerBrandon(),
            _ => new YoungsterJoey(),
        };
    }
}

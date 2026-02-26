public enum Move
{
    None,
    Tackle,
    Bite,
    QuickAttack,
    Bubble,
    Lick,
    Zap,
}

public static class MoveExtensions
{
    public static string GetName(this Move move)
    {
        return move switch
        {
            Move.None => "-",
            Move.Tackle => "Tackle",
            Move.Bite => "Bite",
            Move.QuickAttack => "Quick Attack",
            Move.Bubble => "Bubble",
            Move.Lick => "Lick",
            Move.Zap => "Zap",
            _ => throw new System.NotImplementedException(),
        };
    }

    public static int GetPower(this Move move)
    {
        return move switch
        {
            Move.None => 0,
            Move.Tackle => 20,
            Move.Bite => 30,
            Move.QuickAttack => 15,
            Move.Bubble => 30,
            Move.Lick => 20,
            Move.Zap => 30,
            _ => throw new System.NotImplementedException(),
        };
    }

    public static TypeElement GetTypeElement(this Move move)
    {
        return move switch
        {
            Move.None => TypeElement.Plain,
            Move.Tackle => TypeElement.Plain,
            Move.Bite => TypeElement.Plain,
            Move.QuickAttack => TypeElement.Plain,
            Move.Bubble => TypeElement.Water,
            Move.Lick => TypeElement.Plain,
            Move.Zap => TypeElement.Thunder,
            _ => throw new System.NotImplementedException(),
        };
    }
}

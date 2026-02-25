public enum Move
{
    None,
    Tackle,
    Bite,
    QuickAttack,
    Bubble,
    Lick,
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
            _ => throw new System.NotImplementedException(),
        };
    }
}

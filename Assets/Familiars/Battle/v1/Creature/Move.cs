public enum Move
{
    None,
    Tackle,
    Bite,
    QuickAttack,
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
            _ => throw new System.NotImplementedException(),
        };
    }
}

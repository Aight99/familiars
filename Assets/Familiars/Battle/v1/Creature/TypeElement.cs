public enum TypeElement
{
    Plain,
    Fire,
    Water,
    Thunder,
    Forest,
}

public static class TypeElementExtensions
{
    public static float GetTypeEffectivenessBonus(
        this TypeElement attackType,
        TypeElement defenderType
    )
    {
        return (attackType, defenderType) switch
        {
            (TypeElement.Fire, TypeElement.Forest) => 2f,
            (TypeElement.Water, TypeElement.Fire) => 2f,
            (TypeElement.Thunder, TypeElement.Water) => 2f,
            (TypeElement.Forest, TypeElement.Thunder) => 2f,
            (TypeElement.Forest, TypeElement.Fire) => 0.5f,
            (TypeElement.Fire, TypeElement.Water) => 0.5f,
            (TypeElement.Water, TypeElement.Thunder) => 0.5f,
            (TypeElement.Thunder, TypeElement.Forest) => 0.5f,
            _ => 1f,
        };
    }
}

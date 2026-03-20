public readonly struct CreatureSpecies
{
    public string Name { get; }
    public TypeElement Type { get; }
    public int Health { get; }
    public int Attack { get; }
    public int Speed { get; }

    public CreatureSpecies(string name, TypeElement type, int health, int attack, int speed)
    {
        Name = name;
        Type = type;
        Health = health;
        Attack = attack;
        Speed = speed;
    }
}

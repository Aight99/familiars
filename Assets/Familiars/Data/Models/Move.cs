public readonly struct Move
{
    public static readonly Move None = new(string.Empty, 0, default, MoveApplicationType.Status);

    public string Name { get; }
    public int Power { get; }
    public TypeElement Type { get; }
    public MoveApplicationType ApplicationType { get; }

    public bool IsNone => string.IsNullOrEmpty(Name);

    public Move(string name, int power, TypeElement type, MoveApplicationType applicationType)
    {
        Name = name;
        Power = power;
        Type = type;
        ApplicationType = applicationType;
    }

    public string DisplayName => IsNone ? "-" : Name;
}

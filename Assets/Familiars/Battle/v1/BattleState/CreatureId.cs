using System;

public readonly struct CreatureId : IEquatable<CreatureId>
{
    private readonly Guid value;

    private CreatureId(Guid value)
    {
        this.value = value;
    }

    public static CreatureId Generate() => new(Guid.NewGuid());

    public bool Equals(CreatureId other) => value == other.value;

    public override bool Equals(object obj) => obj is CreatureId other && Equals(other);

    public override int GetHashCode() => value.GetHashCode();

    public static bool operator ==(CreatureId a, CreatureId b) => a.Equals(b);

    public static bool operator !=(CreatureId a, CreatureId b) => !a.Equals(b);
}

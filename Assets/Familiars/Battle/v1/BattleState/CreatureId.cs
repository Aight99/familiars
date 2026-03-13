using System;
using UnityEngine;

[Serializable]
public struct CreatureId : IEquatable<CreatureId>
{
    [SerializeField]
    private string value;

    private CreatureId(string value)
    {
        this.value = value;
    }

    public readonly bool IsEmpty => string.IsNullOrEmpty(value);

    public static CreatureId Generate() => new(Guid.NewGuid().ToString());

    public bool Equals(CreatureId other) => value == other.value;

    public override bool Equals(object obj) => obj is CreatureId other && Equals(other);

    public override readonly int GetHashCode() => value?.GetHashCode() ?? 0;

    public static bool operator ==(CreatureId a, CreatureId b) => a.Equals(b);

    public static bool operator !=(CreatureId a, CreatureId b) => !a.Equals(b);
}

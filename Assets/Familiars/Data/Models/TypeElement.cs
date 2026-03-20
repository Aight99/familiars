using System.Collections.Generic;

public readonly struct TypeElement
{
    public string Name { get; }
    public IReadOnlyList<string> EffectiveAgainst { get; }
    public IReadOnlyList<string> IneffectiveAgainst { get; }

    public TypeElement(
        string name,
        IReadOnlyList<string> effectiveAgainst,
        IReadOnlyList<string> ineffectiveAgainst
    )
    {
        Name = name;
        EffectiveAgainst = effectiveAgainst;
        IneffectiveAgainst = ineffectiveAgainst;
    }

    public float GetEffectivenessBonus(TypeElement defender)
    {
        if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(defender.Name))
            return 1f;

        foreach (var entry in EffectiveAgainst)
        {
            if (entry == defender.Name)
                return 2f;
        }

        foreach (var entry in IneffectiveAgainst)
        {
            if (entry == defender.Name)
                return 0.5f;
        }

        return 1f;
    }
}

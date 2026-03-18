using System;

[Serializable]
public class CreatureSpeciesEntry
{
    public string icon = "";
    public string name = "";
    public string type = "";
    public CreatureStatsEntry stats = new();
}

[Serializable]
public class CreatureStatsEntry
{
    public float attack;
    public float health;
    public float speed;
}

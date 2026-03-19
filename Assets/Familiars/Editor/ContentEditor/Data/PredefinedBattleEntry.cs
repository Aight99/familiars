using System;
using System.Collections.Generic;

[Serializable]
public class PredefinedBattleEntry
{
    public string name = "";
    public List<PredefinedCreatureEntry> creatures = new();
}

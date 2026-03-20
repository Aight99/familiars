using System;
using System.Collections.Generic;

[Serializable]
public class BattleTeamEntry
{
    public string name = "";
    public List<BattleTeamCreatureEntry> creatures = new();
}

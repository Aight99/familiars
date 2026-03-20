using System.Collections.Generic;
using UnityEngine;

public class BattleState
{
    private readonly Dictionary<CreatureId, Creature> creatures = new();

    public CreatureId PlayerCreatureId { get; }
    public CreatureId RivalCreatureId { get; }
    public string RivalTeamName { get; }
    public int TurnCount { get; private set; }

    private BattleState(Creature playerCreature, Creature rivalCreature, string rivalTeamName)
    {
        creatures[playerCreature.Id] = playerCreature;
        creatures[rivalCreature.Id] = rivalCreature;
        PlayerCreatureId = playerCreature.Id;
        RivalCreatureId = rivalCreature.Id;
        RivalTeamName = rivalTeamName;
        TurnCount = 1;
    }

    public static BattleState FromBattleConfig(BattleConfig battleConfig)
    {
        var playerTeam = battleConfig.PlayerTeam;
        var rivalTeam = battleConfig.RivalTeam;

        if (playerTeam == null || playerTeam.Creatures == null || playerTeam.Creatures.Count == 0)
        {
            Debug.LogError("BattleState: player BattleTeam is missing or has no creatures.");
            return null;
        }

        if (rivalTeam == null || rivalTeam.Creatures == null || rivalTeam.Creatures.Count == 0)
        {
            Debug.LogError("BattleState: rival BattleTeam is missing or has no creatures.");
            return null;
        }

        var playerEntry = playerTeam.Creatures[0];
        var rivalEntry = rivalTeam.Creatures[0];
        return new BattleState(
            CreateCreatureFromTeamCreature(playerEntry),
            CreateCreatureFromTeamCreature(rivalEntry),
            rivalTeam.Name
        );
    }

    public Creature GetCreature(CreatureId id) => creatures[id];

    public void IncrementTurn()
    {
        TurnCount++;
    }

    private static Creature CreateCreatureFromTeamCreature(BattleTeamCreature entry)
    {
        var id = CreatureId.FromString(entry.CreatureId);
        return new Creature(
            id,
            entry.Species.Name,
            entry.Species.Type,
            entry.Species.Health,
            entry.Species.Attack,
            entry.Species.Speed,
            entry.Moves
        );
    }
}

using System;
using UnityEngine;

public class OverworldCreature : MonoBehaviour
{
    [SerializeField]
    private string battleTeamName;

    [SerializeField]
    private GameDataProvider gameDataProvider;

    private Action<BattleTeam> onPlayerEncountered;

    public string TeamName => battleTeamName;

    public void Initialize(Action<BattleTeam> onPlayerEncountered)
    {
        this.onPlayerEncountered = onPlayerEncountered;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Player>(out _))
            return;

        if (gameDataProvider == null)
        {
            Debug.LogError("OverworldCreature: GameDataProvider is not assigned.");
            return;
        }

        var team = gameDataProvider.Service.GetBattleTeam(battleTeamName);
        if (team == null)
        {
            Debug.LogError($"OverworldCreature: unknown battle team '{battleTeamName}'.");
            return;
        }

        onPlayerEncountered?.Invoke(team);
    }
}

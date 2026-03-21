using System;
using UnityEngine;

public class OverworldCreature : MonoBehaviour
{
    [SerializeField]
    private string battleTeamName;

    private Action<string> onPlayerEncountered;

    public string TeamName => battleTeamName;

    public void Initialize(string teamName, Action<string> onPlayerEncountered)
    {
        battleTeamName = teamName;
        this.onPlayerEncountered = onPlayerEncountered;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Player>(out _))
            return;

        onPlayerEncountered?.Invoke(battleTeamName);
    }
}

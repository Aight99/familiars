using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private SceneLoader sceneLoader;

    [SerializeField]
    private CreatureSpawner creatureSpawner;

    [SerializeField]
    private Transform playerRespawnPoint;

    private bool canStartBattle = true;

    public void OnCreatureEncountered(BattleTeam rivalTeam)
    {
        if (!canStartBattle || rivalTeam == null)
            return;

        var playerTeam = player.Team;
        if (playerTeam == null)
        {
            Debug.LogError("OverworldManager: player BattleTeam is null.");
            return;
        }

        canStartBattle = false;
        var config = new BattleConfig(playerTeam, rivalTeam);
        sceneLoader.OpenBattleScene(config, OnBattleEnded);
    }

    private void OnBattleEnded(BattleResult result)
    {
        if (result.isPlayerWon)
            creatureSpawner.DestroyCreature(result.rivalTeamName);
        else
        {
            player.transform.SetPositionAndRotation(
                playerRespawnPoint.position,
                playerRespawnPoint.rotation
            );
        }

        canStartBattle = true;
    }
}

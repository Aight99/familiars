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

    private GameDataService gameDataService;
    private bool canStartBattle = true;

    public void Initialize(GameDataService service)
    {
        gameDataService = service;
    }

    public void OnCreatureEncountered(string rivalTeamName)
    {
        if (!canStartBattle || string.IsNullOrEmpty(rivalTeamName))
            return;

        if (gameDataService == null)
        {
            Debug.LogError("OverworldManager: GameDataService is not initialized.");
            return;
        }

        var playerTeam = gameDataService.GetBattleTeam(player.BattleTeamName);
        if (playerTeam == null)
        {
            Debug.LogError("OverworldManager: player BattleTeam is null.");
            return;
        }

        var rivalTeam = gameDataService.GetBattleTeam(rivalTeamName);
        if (rivalTeam == null)
        {
            Debug.LogError($"OverworldManager: unknown rival battle team '{rivalTeamName}'.");
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
            player.TeleportTo(playerRespawnPoint);
        }

        canStartBattle = true;
    }
}

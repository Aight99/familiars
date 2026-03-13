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

    public void OnCreatureEncountered(PredefinedCreature rivalCreature)
    {
        if (!canStartBattle)
            return;
        canStartBattle = false;
        sceneLoader.OpenBattleScene(player.Partner, rivalCreature, OnBattleEnded);
    }

    private void OnBattleEnded(BattleResult result)
    {
        if (result.isPlayerWon)
        {
            creatureSpawner.DestroyCreature(result.rivalCreatureId);
        }
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

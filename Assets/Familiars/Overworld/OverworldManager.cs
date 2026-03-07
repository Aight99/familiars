using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private SceneLoader sceneLoader;

    public void OnCreatureEncountered(PredefinedCreature rivalCreature)
    {
        sceneLoader.OpenBattleScene(player.Partner, rivalCreature);
    }
}

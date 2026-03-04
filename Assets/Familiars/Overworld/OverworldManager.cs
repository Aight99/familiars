using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    [SerializeField]
    private Player player;

    public void OnCreatureEncountered(PredefinedCreature rivalCreature)
    {
        SceneLoader.Instance.LoadBattleScene(player.Partner, rivalCreature);
    }
}

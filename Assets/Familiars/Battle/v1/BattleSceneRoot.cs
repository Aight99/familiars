using UnityEngine;

public class BattleSceneRoot : MonoBehaviour
{
    [SerializeField]
    private BattleUI battleUI;

    [SerializeField]
    private BattleViewManager battleViewManager;

    [SerializeField]
    private BattleManager battleManager;

    public void Initialize(
        BattleConfig config,
        BattleSceneHandler handler,
        CreaturePrefabRegistry creaturePrefabRegistry
    )
    {
        battleUI.Initialize();
        battleViewManager.Initialize(creaturePrefabRegistry);
        battleManager.Initialize(config, handler);
        var state = battleManager.BattleState;
        if (state != null)
            battleViewManager.UpdateWithState(state);
    }
}

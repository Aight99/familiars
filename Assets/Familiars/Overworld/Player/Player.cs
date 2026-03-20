using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private string battleTeamName;

    [SerializeField]
    private GameDataProvider gameDataProvider;

    public BattleTeam Team =>
        gameDataProvider != null ? gameDataProvider.Service.GetBattleTeam(battleTeamName) : null;
}

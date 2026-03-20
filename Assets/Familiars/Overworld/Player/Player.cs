using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private string battleTeamName;

    public string BattleTeamName => battleTeamName;
}

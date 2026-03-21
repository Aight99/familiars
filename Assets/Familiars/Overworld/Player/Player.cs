using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private string battleTeamName;

    private PlayerMovement movement;

    public string BattleTeamName => battleTeamName;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    public void TeleportTo(Transform destination)
    {
        movement.TeleportTo(destination.position, destination.rotation);
    }
}

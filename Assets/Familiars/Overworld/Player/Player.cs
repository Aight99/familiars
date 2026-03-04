using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private PredefinedCreature partner;

    public PredefinedCreature Partner => partner;
}

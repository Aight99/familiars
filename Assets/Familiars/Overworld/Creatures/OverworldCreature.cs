using UnityEngine;

public class OverworldCreature : MonoBehaviour
{
    [SerializeField]
    private PredefinedCreature model;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out _))
        {
            Debug.Log($"Creature {name} encountered the player");
        }
    }
}

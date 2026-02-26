using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField]
    private Transform playerCreaturePosition;

    [SerializeField]
    private Transform rivalCreaturePosition;

    public void PlaceCreatures(GameObject playerModel, GameObject rivalModel)
    {
        Instantiate(playerModel, playerCreaturePosition);
        Instantiate(rivalModel, rivalCreaturePosition);
    }
}

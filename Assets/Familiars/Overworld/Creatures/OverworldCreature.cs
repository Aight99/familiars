using UnityEngine;

public class OverworldCreature : MonoBehaviour
{
    [SerializeField]
    private PredefinedCreature model;

    // TODO: Заменить на коллбек после перехода на динамическое создание существ
    [SerializeField]
    private OverworldManager overworldManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out _))
        {
            overworldManager.OnCreatureEncountered(model);
        }
    }
}

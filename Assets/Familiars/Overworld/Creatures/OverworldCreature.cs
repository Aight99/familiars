using System;
using UnityEngine;

public class OverworldCreature : MonoBehaviour
{
    [SerializeField]
    private PredefinedCreature model;

    private Action<PredefinedCreature> onPlayerEncountered;

    public void Initialize(Action<PredefinedCreature> onPlayerEncountered)
    {
        this.onPlayerEncountered = onPlayerEncountered;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out _))
        {
            onPlayerEncountered?.Invoke(model);
        }
    }
}

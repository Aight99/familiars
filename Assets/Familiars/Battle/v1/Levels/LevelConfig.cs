using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Familiars/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [SerializeField]
    private PredefinedCreature playerCreature;

    [SerializeField]
    private PredefinedCreature rivalCreature;

    public PredefinedCreature PlayerCreature => playerCreature;
    public PredefinedCreature RivalCreature => rivalCreature;
}

using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Familiars/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [SerializeField]
    private Creature playerCreature;

    [SerializeField]
    private Creature rivalCreature;

    public Creature PlayerCreature => playerCreature;
    public Creature RivalCreature => rivalCreature;
}

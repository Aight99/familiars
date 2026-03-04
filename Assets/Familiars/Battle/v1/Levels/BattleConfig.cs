using UnityEngine;

[CreateAssetMenu(fileName = "BattleConfig", menuName = "Familiars/BattleConfig")]
public class BattleConfig : ScriptableObject
{
    [SerializeField]
    private PredefinedCreature playerCreature;

    [SerializeField]
    private PredefinedCreature rivalCreature;

    public PredefinedCreature PlayerCreature => playerCreature;
    public PredefinedCreature RivalCreature => rivalCreature;

    public void Setup(PredefinedCreature playerCreature, PredefinedCreature rivalCreature)
    {
        this.playerCreature = playerCreature;
        this.rivalCreature = rivalCreature;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "CreatureKind", menuName = "Familiars/Creature Kind")]
public class CreatureKind : ScriptableObject
{
    [SerializeField]
    private string kindName;

    [SerializeField]
    private TypeElement type;

    [SerializeField]
    private int health;

    [SerializeField]
    private int attack;

    [SerializeField]
    private int speed;

    public string KindName => kindName;
    public TypeElement Type => type;
    public int Health => health;
    public int Attack => attack;
    public int Speed => speed;
}

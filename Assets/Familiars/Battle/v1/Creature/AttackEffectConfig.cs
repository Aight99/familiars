using UnityEngine;

[CreateAssetMenu(fileName = "AttackEffect", menuName = "Familiars/Attack Effect")]
public class AttackEffectConfig : ScriptableObject
{
    [SerializeField]
    private GameObject vfxPrefab;

    public GameObject VfxPrefab => vfxPrefab;
}

using UnityEngine;

public static class AnimatonTriggers
{
    public static readonly int AttackPhysical = Animator.StringToHash("AttackPhysical");
    public static readonly int AttackRanged = Animator.StringToHash("AttackRanged");
    public static readonly int AttackStatus = Animator.StringToHash("AttackStatus");
    public static readonly int TakeHit = Animator.StringToHash("TakeHit");
    public static readonly int Faint = Animator.StringToHash("Faint");
}

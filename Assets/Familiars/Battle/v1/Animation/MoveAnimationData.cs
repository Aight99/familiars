public readonly struct MoveAnimationData
{
    public readonly CreatureId UserId;
    public readonly CreatureId TargetId;
    public readonly MoveApplicationType ApplicationType;
    public readonly AttackEffectConfig AttackEffect;

    public MoveAnimationData(
        CreatureId userId,
        CreatureId targetId,
        MoveApplicationType applicationType,
        AttackEffectConfig attackEffect
    )
    {
        UserId = userId;
        TargetId = targetId;
        ApplicationType = applicationType;
        AttackEffect = attackEffect;
    }
}

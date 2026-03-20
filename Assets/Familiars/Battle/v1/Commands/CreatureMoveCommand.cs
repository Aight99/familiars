using UnityEngine;

readonly struct CreatureMoveCommand : ICommand
{
    private readonly CreatureId userId;
    private readonly CreatureId targetId;
    private readonly Move move;

    public CreatureMoveCommand(CreatureId userId, CreatureId targetId, Move move)
    {
        this.userId = userId;
        this.targetId = targetId;
        this.move = move;
    }

    public readonly void Execute(BattleState state)
    {
        if (move.IsNone)
            return;

        var user = state.GetCreature(userId);
        var target = state.GetCreature(targetId);
        var damage = CalculateDamage(user, target);
        target.ApplyDamage(damage);
    }

    public readonly int GetPriority(BattleState state)
    {
        return state.GetCreature(userId).Speed;
    }

    public readonly MoveAnimationData? GetAnimationData(BattleState state)
    {
        if (move.IsNone)
            return null;

        return new MoveAnimationData(userId, targetId, move.ApplicationType, null);
    }

    private int CalculateDamage(Creature user, Creature target)
    {
        var randomModifier = Random.Range(0.85f, 1.00f);
        var typeBonus = move.Type.GetEffectivenessBonus(target.Type);
        var modifier = randomModifier * typeBonus;
        var damage = ((user.Attack * move.Power) / 50f + 2) * modifier;
        return Mathf.Max(1, Mathf.FloorToInt(damage));
    }
}

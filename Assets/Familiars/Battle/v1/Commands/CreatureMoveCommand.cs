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
        var user = state.GetCreature(userId);
        var target = state.GetCreature(targetId);
        var damage = CalculateDamage(user, target);
        target.ApplyDamage(damage);
        DebugHelper.Log(
            DebugHelper.MessageType.Other,
            $"{user.Kind.KindName} used {move.GetName()} and dealt {damage} damage to {target.Kind.KindName}"
        );
        if (target.IsFainted)
        {
            DebugHelper.Log(DebugHelper.MessageType.Fiasco, $"{target.Kind.KindName} fainted!");
        }
    }

    public readonly int GetPriority(BattleState state)
    {
        return state.GetCreature(userId).Speed;
    }

    private int CalculateDamage(Creature user, Creature target)
    {
        var randomModifier = Random.Range(0.85f, 1.00f);
        var typeBonus = move.GetTypeElement().GetTypeEffectivenessBonus(target.Type);
        var modifier = randomModifier * typeBonus;
        var damage = ((user.Attack * move.GetPower()) / 50f + 2) * modifier;
        return Mathf.Max(1, Mathf.FloorToInt(damage));
    }
}

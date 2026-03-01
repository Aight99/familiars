public interface ICommand
{
    void Execute(BattleState state);
    int GetPriority(BattleState state);
    MoveAnimationData? GetAnimationData(BattleState state);
}

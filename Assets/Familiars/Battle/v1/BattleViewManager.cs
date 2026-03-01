using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleViewManager : MonoBehaviour
{
    [SerializeField]
    private AttackButtonsPanel attackButtonsPanel;

    [SerializeField]
    private Field field;

    [SerializeField]
    private BattleAnimationSequencer animationSequencer;

    public event Action<Move> OnMoveSelected;

    private readonly Dictionary<CreatureId, CreatureView> creatureViews = new();

    private void Awake()
    {
        attackButtonsPanel.OnMoveSelected += HandleMoveSelected;
    }

    private void OnDestroy()
    {
        attackButtonsPanel.OnMoveSelected -= HandleMoveSelected;
    }

    private void HandleMoveSelected(Move move)
    {
        OnMoveSelected?.Invoke(move);
    }

    public void UpdateWithState(BattleState state)
    {
        SetPlayerMoves(state.GetCreature(state.PlayerCreatureId).Moves);
        PlaceCreatures(state);
    }

    private void SetPlayerMoves(IReadOnlyList<Move> moves)
    {
        attackButtonsPanel.SetMoves(moves);
    }

    private void PlaceCreatures(BattleState state)
    {
        var playerCreature = state.GetCreature(state.PlayerCreatureId);
        var rivalCreature = state.GetCreature(state.RivalCreatureId);

        var (playerView, rivalView) = field.PlaceCreatures(playerCreature, rivalCreature);

        creatureViews[state.PlayerCreatureId] = playerView;
        creatureViews[state.RivalCreatureId] = rivalView;
    }

    public CreatureView GetCreatureView(CreatureId id) => creatureViews[id];

    public async Task PlayAnimation(MoveAnimationData data, Action onHit)
    {
        await animationSequencer.Play(
            data,
            creatureViews[data.UserId],
            creatureViews[data.TargetId],
            onHit
        );
    }

    public void SetAttackButtonsVisible(bool visible)
    {
        attackButtonsPanel.SetVisible(visible);
    }
}

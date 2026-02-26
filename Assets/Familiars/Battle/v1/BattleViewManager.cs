using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleViewManager : MonoBehaviour
{
    [SerializeField]
    private AttackButtonsPanel attackButtonsPanel;

    [SerializeField]
    private Field field;

    public event Action<Move> OnMoveSelected;

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
        UpdateField(state);
    }

    private void SetPlayerMoves(IReadOnlyList<Move> moves)
    {
        attackButtonsPanel.SetMoves(moves);
    }

    private void UpdateField(BattleState state)
    {
        field.PlaceCreatures(
            state.GetCreature(state.PlayerCreatureId).Kind.Model,
            state.GetCreature(state.RivalCreatureId).Kind.Model
        );
    }

    public void SetAttackButtonsVisible(bool visible)
    {
        attackButtonsPanel.SetVisible(visible);
    }
}

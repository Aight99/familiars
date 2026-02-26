using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleViewManager : MonoBehaviour
{
    [SerializeField]
    private AttackButtonsPanel attackButtonsPanel;

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

    public void SetPlayerMoves(IReadOnlyList<Move> moves)
    {
        attackButtonsPanel.SetMoves(moves);
    }

    public void SetAttackButtonsVisible(bool visible)
    {
        attackButtonsPanel.SetVisible(visible);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleViewManager : MonoBehaviour
{
    [SerializeField]
    private BattleUI battleUI;

    [SerializeField]
    private Field field;

    [SerializeField]
    private BattleAnimationSequencer animationSequencer;

    public event Action<Move> OnMoveSelected;

    private readonly Dictionary<CreatureId, CreatureView> creatureViews = new();

    private void Awake()
    {
        battleUI.OnMoveSelected += HandleMoveSelected;
    }

    private void OnDestroy()
    {
        battleUI.OnMoveSelected -= HandleMoveSelected;
    }

    private void HandleMoveSelected(Move move)
    {
        OnMoveSelected?.Invoke(move);
    }

    public void UpdateWithState(BattleState state)
    {
        UpdateUI(state);
        PlaceCreatures(state);
    }

    public void UpdateUI(BattleState state)
    {
        var playerCreature = state.GetCreature(state.PlayerCreatureId);
        var rivalCreature = state.GetCreature(state.RivalCreatureId);

        battleUI.SetMoves(playerCreature.Moves);
        battleUI.UpdatePlayerHealth(
            playerCreature.Kind.KindName,
            playerCreature.Health,
            playerCreature.MaxHealth
        );
        battleUI.UpdateRivalHealth(
            rivalCreature.Kind.KindName,
            rivalCreature.Health,
            rivalCreature.MaxHealth
        );
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
        battleUI.SetAttackButtonsVisible(visible);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    private BattleViewManager battleViewManager;

    private BattleSceneHandler battleSceneHandler;

    private BattleState battleState;
    private readonly List<ICommand> commandOrderQueue = new();
    private IRivalAi rivalAi;

    private bool moveSelectionSubscribed;

    public BattleState BattleState => battleState;

    private void OnDestroy()
    {
        if (moveSelectionSubscribed)
            battleViewManager.OnMoveSelected -= OnMoveSelected;
    }

    public void Initialize(BattleConfig config, BattleSceneHandler handler)
    {
        battleSceneHandler = handler;
        if (config == null)
        {
            Debug.LogError("BattleManager: BattleConfig is null.");
            return;
        }

        var state = BattleState.FromBattleConfig(config);
        if (state == null)
        {
            Debug.LogError("BattleManager: failed to create BattleState.");
            return;
        }

        battleState = state;
        rivalAi = RivalAiFactory.Create();
        commandOrderQueue.Clear();
        battleViewManager.OnMoveSelected += OnMoveSelected;
        moveSelectionSubscribed = true;
    }

    private void OnMoveSelected(Move move)
    {
        if (battleState == null)
            return;

        SendCommand(
            new CreatureMoveCommand(battleState.PlayerCreatureId, battleState.RivalCreatureId, move)
        );
    }

    public void SendCommand(ICommand command)
    {
        if (battleState == null)
            return;

        battleViewManager.SetAttackButtonsVisible(false);

        commandOrderQueue.Add(command);
        commandOrderQueue.Add(rivalAi.GetCommand(battleState));

        _ = ExecuteCommands();
    }

    public async Task ExecuteCommands()
    {
        if (battleState == null)
            return;

        while (commandOrderQueue.Count > 0 && !IsAnyCreatureFainted())
        {
            ShuffleCommands();
            SortCommandsByPriorityDescending();

            var command = commandOrderQueue[0];
            commandOrderQueue.RemoveAt(0);

            void ExecuteAndUpdateUI()
            {
                command.Execute(battleState);
                battleViewManager.UpdateUI(battleState);
            }

            var animationData = command.GetAnimationData(battleState);
            if (animationData.HasValue)
            {
                await battleViewManager.PlayAnimation(animationData.Value, ExecuteAndUpdateUI);
            }
            else
            {
                ExecuteAndUpdateUI();
            }
        }

        if (IsAnyCreatureFainted())
        {
            var rivalFainted = battleState.GetCreature(battleState.RivalCreatureId).IsFainted;
            StartCoroutine(HandleBattleEndCoroutine(rivalFainted));
            return;
        }

        battleState.IncrementTurn();
        battleViewManager.SetAttackButtonsVisible(true);
    }

    private bool IsAnyCreatureFainted()
    {
        if (battleState == null)
            return false;

        return battleState.GetCreature(battleState.PlayerCreatureId).IsFainted
            || battleState.GetCreature(battleState.RivalCreatureId).IsFainted;
    }

    private IEnumerator HandleBattleEndCoroutine(bool playerWon)
    {
        yield return new WaitForSeconds(1.5f);
        var battleResult = new BattleResult
        {
            isPlayerWon = playerWon,
            rivalTeamName = battleState.RivalTeamName,
        };
        battleSceneHandler.OnBattleEnd?.Invoke(battleResult);
    }

    private void ShuffleCommands()
    {
        for (var i = 0; i < commandOrderQueue.Count; i++)
        {
            var randomIndex = Random.Range(i, commandOrderQueue.Count);
            (commandOrderQueue[i], commandOrderQueue[randomIndex]) = (
                commandOrderQueue[randomIndex],
                commandOrderQueue[i]
            );
        }
    }

    private void SortCommandsByPriorityDescending()
    {
        commandOrderQueue.Sort(
            (a, b) => b.GetPriority(battleState).CompareTo(a.GetPriority(battleState))
        );
    }
}

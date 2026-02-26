using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    private LevelConfig levelConfig;

    [SerializeField]
    private BattleViewManager battleViewManager;

    private BattleState battleState;
    private readonly List<ICommand> commandOrderQueue = new();
    private IRivalAi rivalAi;

    private void Awake()
    {
        battleState = BattleState.FromLevelConfig(levelConfig);
        rivalAi = RivalAiFactory.Create();

        battleViewManager.SetPlayerMoves(battleState.PlayerCreature.Moves);
        battleViewManager.OnMoveSelected += OnMoveSelected;
    }

    private void OnDestroy()
    {
        battleViewManager.OnMoveSelected -= OnMoveSelected;
    }

    private void OnMoveSelected(Move move)
    {
        SendCommand(new DummyCommand(move.GetName()));
    }

    public void SendCommand(ICommand command)
    {
        battleViewManager.SetAttackButtonsVisible(false);
        DebugHelper.Log(DebugHelper.MessageType.Other, $"Turn {battleState.TurnCount} started!");

        commandOrderQueue.Add(command);
        commandOrderQueue.Add(rivalAi.GetCommand(battleState));

        _ = ExecuteCommands();
    }

    public async Task ExecuteCommands()
    {
        while (commandOrderQueue.Count > 0)
        {
            await WaitOneSecond();

            ShuffleCommands();
            SortCommandsByPriorityDescending();

            var command = commandOrderQueue[0];
            commandOrderQueue.RemoveAt(0);
            command.Execute();
        }

        await WaitOneSecond();
        battleState.IncrementTurn();
        battleViewManager.SetAttackButtonsVisible(true);
    }

    private async Task WaitOneSecond()
    {
        await Task.Delay(1000);
    }

    private void ShuffleCommands()
    {
        for (int i = 0; i < commandOrderQueue.Count; i++)
        {
            int randomIndex = Random.Range(i, commandOrderQueue.Count);
            (commandOrderQueue[i], commandOrderQueue[randomIndex]) = (
                commandOrderQueue[randomIndex],
                commandOrderQueue[i]
            );
        }
    }

    private void SortCommandsByPriorityDescending()
    {
        commandOrderQueue.Sort((a, b) => b.GetPriority().CompareTo(a.GetPriority()));
    }
}

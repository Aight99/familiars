using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    private LevelConfig levelConfig;

    [SerializeField]
    private AttackButtonsPanel attackButtonsPanel;

    private BattleState battleState;
    private readonly List<ICommand> commandOrderQueue = new();
    private IRivalAi rivalAi;
    private bool canReceiveCommands = true;

    private void Awake()
    {
        battleState = BattleState.FromLevelConfig(levelConfig);
        rivalAi = RivalAiFactory.Create();

        attackButtonsPanel.SetMoves(battleState.PlayerCreature.Moves);
        attackButtonsPanel.OnMoveSelected += OnMoveSelected;
    }

    private void OnDestroy()
    {
        attackButtonsPanel.OnMoveSelected -= OnMoveSelected;
    }

    private void OnMoveSelected(Move move)
    {
        SendCommand(new DummyCommand(move.GetName()));
    }

    public void SendCommand(ICommand command)
    {
        if (!canReceiveCommands)
            return;

        canReceiveCommands = false;
        DebugHelper.Log(DebugHelper.MessageType.Other, $"Turn {battleState.TurnCount} started!");

        commandOrderQueue.Add(command);
        commandOrderQueue.Add(rivalAi.GetCommand(battleState));

        _ = ExecuteCommands();
    }

    public async Task ExecuteCommands()
    {
        while (commandOrderQueue.Count > 0)
        {
            await Task.Delay(1000);

            ShuffleCommands();
            SortCommandsByPriorityDescending();

            var command = commandOrderQueue[0];
            commandOrderQueue.RemoveAt(0);
            command.Execute();
        }
        battleState.IncrementTurn();
        canReceiveCommands = true;
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

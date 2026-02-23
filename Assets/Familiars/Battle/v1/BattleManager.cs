using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private readonly List<ICommand> commandOrderQueue = new();
    private IRivalAi rivalAi;
    private bool canReceiveCommands = true;

    private void Awake()
    {
        rivalAi = RivalAiFactory.Create();
    }

    public void SendCommand(ICommand command)
    {
        if (!canReceiveCommands)
            return;

        canReceiveCommands = false;
        DebugHelper.Log(DebugHelper.MessageType.Other, $"Command Sent!");

        commandOrderQueue.Add(command);
        commandOrderQueue.Add(rivalAi.GetCommand());

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
        canReceiveCommands = true;
        DebugHelper.Log(DebugHelper.MessageType.Other, $"Execution Complete!");
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

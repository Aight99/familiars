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
        battleViewManager.OnMoveSelected += OnMoveSelected;
    }

    private void Start()
    {
        battleViewManager.UpdateWithState(battleState);
    }

    private void OnDestroy()
    {
        battleViewManager.OnMoveSelected -= OnMoveSelected;
    }

    private void OnMoveSelected(Move move)
    {
        SendCommand(
            new CreatureMoveCommand(battleState.PlayerCreatureId, battleState.RivalCreatureId, move)
        );
    }

    public void SendCommand(ICommand command)
    {
        battleViewManager.SetAttackButtonsVisible(false);

        commandOrderQueue.Add(command);
        commandOrderQueue.Add(rivalAi.GetCommand(battleState));

        _ = ExecuteCommands();
    }

    public async Task ExecuteCommands()
    {
        while (commandOrderQueue.Count > 0)
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
                await battleViewManager.PlayAnimation(
                    animationData.Value,
                    // Эффект отработает в момент соприкосновения
                    ExecuteAndUpdateUI
                );
            }
            else
            {
                ExecuteAndUpdateUI();
            }
        }

        battleState.IncrementTurn();
        battleViewManager.SetAttackButtonsVisible(true);
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
        commandOrderQueue.Sort(
            (a, b) => b.GetPriority(battleState).CompareTo(a.GetPriority(battleState))
        );
    }
}

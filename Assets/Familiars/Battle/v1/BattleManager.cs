using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    private BattleConfig fallbackBattleConfig;

    [SerializeField]
    private BattleViewManager battleViewManager;

    private BattleSceneHandler battleSceneHandler;

    private BattleState battleState;
    private readonly List<ICommand> commandOrderQueue = new();
    private IRivalAi rivalAi;
    private bool initialized;
    private bool started;

    private void Awake()
    {
        battleViewManager.OnMoveSelected += OnMoveSelected;
    }

    // TODO: Слишком сложная логика с initialized/started
    private void Start()
    {
        started = true;

        if (!initialized)
            Initialize(fallbackBattleConfig, default);
        else
            battleViewManager.UpdateWithState(battleState);
    }

    private void OnEnable()
    {
        if (!initialized || !started)
            return;

        battleViewManager.UpdateWithState(battleState);
    }

    private void OnDestroy()
    {
        battleViewManager.OnMoveSelected -= OnMoveSelected;
    }

    public void Initialize(BattleConfig config, BattleSceneHandler handler)
    {
        initialized = true;
        battleSceneHandler = handler;
        battleState = BattleState.FromBattleConfig(config);
        rivalAi = RivalAiFactory.Create();
        commandOrderQueue.Clear();
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

    private bool IsAnyCreatureFainted() =>
        battleState.GetCreature(battleState.PlayerCreatureId).IsFainted
        || battleState.GetCreature(battleState.RivalCreatureId).IsFainted;

    private IEnumerator HandleBattleEndCoroutine(bool playerWon)
    {
        yield return new WaitForSeconds(1.5f);
        battleSceneHandler.OnBattleEnd?.Invoke();
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

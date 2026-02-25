using System.Collections.Generic;
using UnityEngine;

public class RandomMoveRivalAi : IRivalAi
{
    public ICommand GetCommand(BattleState currentState)
    {
        var availableMoves = new List<Move>();

        foreach (var move in currentState.RivalCreature.Moves)
        {
            if (move != Move.None)
                availableMoves.Add(move);
        }

        var selectedMove = availableMoves[Random.Range(0, availableMoves.Count)];
        return new DummyCommand(selectedMove.GetName());
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackButtonsPanel
{
    public event Action<Move> OnMoveSelected;

    private readonly VisualElement grid;
    private readonly Button[] buttons = new Button[4];
    private readonly Move[] currentMoves = new Move[4];

    public AttackButtonsPanel(UIDocument uiDocument)
    {
        var root = uiDocument.rootVisualElement;

        grid = new VisualElement();
        grid.AddToClassList("button-grid");

        for (var row = 0; row < 2; row++)
        {
            var buttonRow = new VisualElement();
            buttonRow.AddToClassList("button-row");

            if (row == 1)
                buttonRow.style.marginBottom = 0;

            for (var col = 0; col < 2; col++)
            {
                var index = row * 2 + col;
                var button = new Button();
                button.AddToClassList("attack-button");

                if (col == 1)
                    button.style.marginRight = 0;

                var captured = index;
                button.clicked += () => OnMoveSelected?.Invoke(currentMoves[captured]);

                buttons[index] = button;
                buttonRow.Add(button);
            }

            grid.Add(buttonRow);
        }

        root.Add(grid);
        ApplyMovesToButtons();
    }

    public void SetVisible(bool visible)
    {
        grid.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void SetMoves(IReadOnlyList<Move> moves)
    {
        if (moves.Count != 4)
        {
            Debug.LogError("moves.Count != 4");
            return;
        }

        var sorted = moves.OrderBy(m => m.IsNone ? 1 : 0).ToList();

        for (var i = 0; i < currentMoves.Length; i++)
            currentMoves[i] = i < sorted.Count ? sorted[i] : Move.None;

        ApplyMovesToButtons();
    }

    private void ApplyMovesToButtons()
    {
        for (var i = 0; i < buttons.Length; i++)
        {
            buttons[i].text = currentMoves[i].DisplayName;
            buttons[i].SetEnabled(!currentMoves[i].IsNone);
        }
    }
}

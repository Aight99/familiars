using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class AttackButtonsPanel : MonoBehaviour
{
    [SerializeField]
    private StyleSheet styleSheet;

    public event Action<Move> OnMoveSelected;

    private VisualElement grid;
    private readonly Button[] buttons = new Button[4];
    private readonly Move[] currentMoves = new Move[4];

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        if (!root.styleSheets.Contains(styleSheet))
            root.styleSheets.Add(styleSheet);

        grid = new VisualElement();
        grid.AddToClassList("button-grid");

        for (int row = 0; row < 2; row++)
        {
            var buttonRow = new VisualElement();
            buttonRow.AddToClassList("button-row");

            if (row == 1)
                buttonRow.style.marginBottom = 0;

            for (int col = 0; col < 2; col++)
            {
                int index = row * 2 + col;
                var button = new Button();
                button.AddToClassList("attack-button");

                if (col == 1)
                    button.style.marginRight = 0;

                int captured = index;
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
        if (grid == null)
            return;

        grid.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void SetMoves(IReadOnlyList<Move> moves)
    {
        if (moves.Count != 4)
        {
            Debug.LogError("moves.Count != 4");
            return;
        }

        var sorted = moves.OrderBy(m => m == Move.None ? 1 : 0).ToList();

        for (int i = 0; i < currentMoves.Length; i++)
            currentMoves[i] = i < sorted.Count ? sorted[i] : Move.None;

        ApplyMovesToButtons();
    }

    private void ApplyMovesToButtons()
    {
        if (buttons[0] == null)
            return;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].text = currentMoves[i].GetName();
            buttons[i].SetEnabled(currentMoves[i] != Move.None);
        }
    }
}

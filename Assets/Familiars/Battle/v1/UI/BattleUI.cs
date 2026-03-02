using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(UIDocument))]
public class BattleUI : MonoBehaviour
{
    [SerializeField]
    private StyleSheet attackButtonsStyleSheet;

    [SerializeField]
    private StyleSheet healthBarStyleSheet;

    public event Action<Move> OnMoveSelected;

    private AttackButtonsPanel attackButtonsPanel;
    private HealthBar playerHealthBar;
    private HealthBar rivalHealthBar;

    private void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        root.styleSheets.Add(attackButtonsStyleSheet);
        root.styleSheets.Add(healthBarStyleSheet);

        attackButtonsPanel = new AttackButtonsPanel(uiDocument);
        attackButtonsPanel.OnMoveSelected += move => OnMoveSelected?.Invoke(move);

        playerHealthBar = new HealthBar(uiDocument, HealthBarAnchor.BottomLeft, true);
        rivalHealthBar = new HealthBar(uiDocument, HealthBarAnchor.TopRight, false);
    }

    public void SetMoves(IReadOnlyList<Move> moves) => attackButtonsPanel.SetMoves(moves);

    public void SetAttackButtonsVisible(bool visible) => attackButtonsPanel.SetVisible(visible);

    public void UpdatePlayerHealth(string creatureName, int current, int max)
    {
        playerHealthBar.SetCreatureName(creatureName);
        playerHealthBar.SetHealth(current, max);
    }

    public void UpdateRivalHealth(string creatureName, int current, int max)
    {
        rivalHealthBar.SetCreatureName(creatureName);
        rivalHealthBar.SetHealth(current, max);
    }
}

using System;
using UnityEngine;
using UnityEngine.UIElements;

// TODO: Полный ужас
[RequireComponent(typeof(UIDocument))]
public class AttackButtonsPanel : MonoBehaviour
{
    private UIDocument uiDocument;

    private Button move1;
    private Button move2;
    private Button move3;
    private Button move4;

    [SerializeField]
    private BattleManager battleManager;

    private Action onMove1Clicked;
    private Action onMove2Clicked;
    private Action onMove3Clicked;
    private Action onMove4Clicked;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();

        var root = uiDocument.rootVisualElement;
        move1 = root.Q<Button>("move1");
        move2 = root.Q<Button>("move2");
        move3 = root.Q<Button>("move3");
        move4 = root.Q<Button>("move4");

        onMove1Clicked = () => battleManager.SendCommand(new DummyCommand("Move 1"));
        onMove2Clicked = () => battleManager.SendCommand(new DummyCommand("Move 2"));
        onMove3Clicked = () => battleManager.SendCommand(new DummyCommand("Move 3"));
        onMove4Clicked = () => battleManager.SendCommand(new DummyCommand("Move 4"));

        if (move1 != null)
            move1.clicked += onMove1Clicked;
        if (move2 != null)
            move2.clicked += onMove2Clicked;
        if (move3 != null)
            move3.clicked += onMove3Clicked;
        if (move4 != null)
            move4.clicked += onMove4Clicked;
    }

    private void OnDisable()
    {
        if (move1 != null)
            move1.clicked -= onMove1Clicked;
        if (move2 != null)
            move2.clicked -= onMove2Clicked;
        if (move3 != null)
            move3.clicked -= onMove3Clicked;
        if (move4 != null)
            move4.clicked -= onMove4Clicked;
    }

    public void SetMoveName(int index, string name)
    {
        var button = GetButton(index);
        if (button != null)
            button.text = name;
    }

    public Button GetButton(int index)
    {
        return index switch
        {
            0 => move1,
            1 => move2,
            2 => move3,
            3 => move4,
            _ => null,
        };
    }
}

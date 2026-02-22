using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public void SendCommand(ICommand command)
    {
        // FIXME: Debug
        command.Execute();
    }
}

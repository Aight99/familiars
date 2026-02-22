public interface ICommand
{
    void Execute();
    int GetPriority();
}

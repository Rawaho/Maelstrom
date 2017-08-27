namespace Shared.Game
{
    public interface IBaseEvent
    {
        bool CanExecute();
        void Execute();
    }
}

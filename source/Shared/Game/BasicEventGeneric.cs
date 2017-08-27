using System;

namespace Shared.Game
{
    public abstract class BasicEventGeneric<T> : IBaseEvent
    {
        protected Action<T> callback;

        public abstract bool CanExecute();
        public abstract void Execute();
    }
}

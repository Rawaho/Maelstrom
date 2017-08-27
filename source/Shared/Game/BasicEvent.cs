using System;

namespace Shared.Game
{
    public abstract class BasicEvent : IBaseEvent
    {
        protected Action callback;

        public abstract bool CanExecute();
        public virtual void Execute()
        {
            callback.Invoke();
        }
    }
}

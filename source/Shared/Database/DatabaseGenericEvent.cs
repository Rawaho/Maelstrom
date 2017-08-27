using System;
using System.Threading.Tasks;
using Shared.Game;

namespace Shared.Database
{
    public class DatabaseGenericEvent<T> : BasicEventGeneric<T>
    {
        private readonly Task<T> task;
        private readonly Action<Exception> callbackException;

        public DatabaseGenericEvent(Task<T> task, Action<T> callback, Action<Exception> callbackException = null)
        {
            this.task              = task;
            this.callback          = callback;
            this.callbackException = callbackException;
        }

        public override bool CanExecute()
        {
            return task.IsCompleted;
        }

        public override void Execute()
        {
            if (task.Exception != null)
                callbackException?.Invoke(task.Exception);
            else
                callback.Invoke(task.Result);
        }
    }
}

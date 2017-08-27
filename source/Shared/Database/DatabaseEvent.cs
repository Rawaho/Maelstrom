using System;
using System.Threading.Tasks;
using Shared.Game;

namespace Shared.Database
{
    public class DatabaseEvent : BasicEvent
    {
        private readonly Task task;
        private readonly Action<Exception> callbackException;

        public DatabaseEvent(Task task, Action callback, Action<Exception> callbackException = null)
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
                base.Execute();
        }
    }
}

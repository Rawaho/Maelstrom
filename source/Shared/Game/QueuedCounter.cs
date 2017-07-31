using System;
using System.Collections.Generic;

namespace Shared.Game
{
    public class QueuedCounter<T> where T : IConvertible
    {
        private T counter;
        private readonly Queue<T> queuedValues = new Queue<T>();
        private readonly object mutex;

        public QueuedCounter(T counter, bool concurrent = false)
        {
            this.counter = counter;
            if (concurrent)
                mutex = new object();
        }

        public T DequeueValue()
        {
            if (mutex == null)
                return _DequeueValue();

            lock (mutex)
                return _DequeueValue();
        }

        private T _DequeueValue()
        {
            if (queuedValues.Count > 0)
                return queuedValues.Dequeue();

            counter = (T)(dynamic)(Convert.ToDouble(counter) + 1d);
            return counter;
        }

        public void EnqueueValue(T value)
        {
            if (mutex != null)
                lock (mutex)
                    queuedValues.Enqueue(value);
            else
                queuedValues.Enqueue(value);
        }
    }
}

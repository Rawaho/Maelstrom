using System;
using System.Collections.Generic;

namespace Shared.Game
{
    public class QueuedCounter<T> where T : IConvertible
    {
        private T counter;
        private readonly Queue<T> queuedValues = new Queue<T>();

        public QueuedCounter(T counter)
        {
            this.counter = counter;
        }

        public T DequeueValue()
        {
            if (queuedValues.Count > 0)
                return queuedValues.Dequeue();

            counter = (T)(dynamic)(Convert.ToDouble(counter) + 1d);
            return counter;
        }

        public void EnqueueValue(T value)
        {
            queuedValues.Enqueue(value);
        }
    }
}

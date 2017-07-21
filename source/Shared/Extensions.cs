using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Shared
{
    public static class Extensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
                return min;
            return val.CompareTo(max) > 0 ? max : val;
        }

        public static bool InRange<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return false;
            return value.CompareTo(max) <= 0;
        }

        public static IEnumerable<T> DequeueMultiple<T>(this ConcurrentQueue<T> queue, uint size)
        {
            for (uint i = 0u; i < size && queue.Count > 0; i++)
                if (queue.TryDequeue(out T value))
                    yield return value;
        }

        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            while (queue.Count > 0)
                queue.TryDequeue(out T value);
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ElBruno.PolarH7
{
    public class FixedSizeQueue<T> : IReadOnlyCollection<T>
    {
        private ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private int _count;

        public int Limit { get; private set; }

        public FixedSizeQueue(int limit)
        {
            this.Limit = limit;
        }

        public void Enqueue(T obj)
        {
            _queue.Enqueue(obj);
            Interlocked.Increment(ref _count);

            // Calculate the number of items to be removed by this thread in a thread safe manner
            int currentCount;
            int finalCount;
            do
            {
                currentCount = _count;
                finalCount = Math.Min(currentCount, this.Limit);
            } while (currentCount !=
                     Interlocked.CompareExchange(ref _count, finalCount, currentCount));

            T overflow;
            while (currentCount > finalCount && _queue.TryDequeue(out overflow))
                currentCount--;
        }

        public int Count
        {
            get { return _count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }
    }
}
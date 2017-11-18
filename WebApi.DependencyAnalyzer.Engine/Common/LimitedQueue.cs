using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.DependencyAnalyzer.Engine.Common
{
    internal class LimitedQueue<TValue> :
        IReadOnlyCollection<TValue>,
        ICollection
    {
        private readonly int _size;
        private readonly Queue<TValue> _queue;

        public LimitedQueue(int size)
        {
            _size = size;
            _queue = new Queue<TValue>(_size);
        }

        public void Enqueue(TValue value)
        {
            if (Count >= _size)
            {
                Dequeue();
            }

            _queue.Enqueue(value);
        }

        public TValue Unenqueue()
        {
            TValue lastElement = default(TValue);

            if (Count > 0)
            {
                TValue[] elements = ToArray();
                Clear();

                foreach (TValue element in elements.Take(elements.Length - 1))
                {
                    Enqueue(element);
                }

                lastElement = elements.Last();
            }

            return lastElement;
        }

        public TValue PeekLast()
        {
            TValue lastElement = default(TValue);

            if (Count > 0)
            {
                TValue[] elements = ToArray();

                lastElement = elements.Last();
            }

            return lastElement;
        }

        public TValue Dequeue() => _queue.Dequeue();

        public TValue Peek() => _queue.Peek();

        public void Clear() => _queue.Clear();

        public bool Contains(TValue value) => _queue.Contains(value);

        public TValue[] ToArray() => _queue.ToArray();

        public IEnumerator<TValue> GetEnumerator() => _queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_queue).GetEnumerator();

        public void CopyTo(Array array, int index) => _queue.CopyTo((TValue[])array, index);

        public int Count => _queue.Count;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();
    }
}
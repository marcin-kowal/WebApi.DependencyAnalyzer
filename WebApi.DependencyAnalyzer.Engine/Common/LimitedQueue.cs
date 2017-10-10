using System;
using System.Collections;
using System.Collections.Generic;

namespace WebApi.DependencyAnalyzer.Engine.Common
{
    internal class LimitedQueue<TValue> : 
        IEnumerable<TValue>, 
        IEnumerable, 
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
            if (_queue.Count >= _size)
            {
                _queue.Dequeue();
            }

            _queue.Enqueue(value);
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
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
        private Func<TValue, TValue, TValue> _appendOperator;
        private Func<TValue, TValue, TValue> _prependOperator;

        public LimitedQueue(int size)
        {
            _size = size;
            _queue = new Queue<TValue>(_size);
        }

        public LimitedQueue<TValue> WithAppendOperator(Func<TValue, TValue, TValue> appendOperator)
        {
            _appendOperator = appendOperator;

            return this;
        }

        public LimitedQueue<TValue> WithPrependOperator(Func<TValue, TValue, TValue> prependOperator)
        {
            _prependOperator = prependOperator;

            return this;
        }

        public void Enqueue(TValue value)
        {
            if (_queue.Count >= _size)
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

        public void AppendToLast(TValue value)
        {
            ModifyLast(value, _appendOperator);
        }

        public void PrependToLast(TValue value)
        {
            ModifyLast(value, _prependOperator);
        }

        internal void ModifyLast(TValue value, Func<TValue, TValue, TValue> modifyOperator)
        {
            if (Count == 0)
            {
                Enqueue(value);
                return;
            }

            if (modifyOperator == null)
            {
                throw new ArgumentNullException(nameof(modifyOperator));
            }

            TValue[] elements = ToArray();
            Clear();

            int lastElementIndex = elements.Length - 1;
            elements[lastElementIndex] = modifyOperator(elements[lastElementIndex], value);

            foreach (TValue element in elements)
            {
                Enqueue(element);
            }
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
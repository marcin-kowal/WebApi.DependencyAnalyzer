using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.DependencyAnalyzer.Engine.Common
{
    internal class LimitedStack<TValue> : IReadOnlyCollection<TValue>
    {
        private readonly int _size;
        private readonly List<TValue> _list;

        public LimitedStack(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentException("Size must be greater than 0", nameof(size));
            }

            _size = size;
            _list = new List<TValue>(_size);
        }

        public void Push(TValue value)
        {
            if (Count >= _size)
            {
                _list.RemoveAt(0);
            }

            _list.Add(value);
        }

        public TValue Pop()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Stack is empty");
            }

            TValue result = _list.Last();

            _list.RemoveAt(_list.Count - 1);

            return result;
        }

        public void Clear() => _list.Clear();

        public IEnumerator<TValue> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();

        public int Count => _list.Count;
    }
}
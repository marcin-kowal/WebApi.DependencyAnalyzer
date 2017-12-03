using System;

namespace WebApi.DependencyAnalyzer.Engine.Common
{
    public class Operation<TValue>
    {
        private readonly Func<TValue, TValue, TValue> _operation;

        public Operation(Func<TValue, TValue, TValue> operation)
        {
            _operation = operation;
        }

        public TValue Execute(TValue first, TValue second)
        {
            return _operation(first, second);
        }
    }
}
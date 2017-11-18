using System;

namespace WebApi.DependencyAnalyzer.Engine.Common
{
    public class Operator<TValue>
    {
        private readonly Func<TValue, TValue, TValue> _operator;

        public Operator(Func<TValue, TValue, TValue> oper)
        {
            _operator = oper;
        }

        public TValue Execute(TValue first, TValue second)
        {
            return _operator(first, second);
        }
    }
}
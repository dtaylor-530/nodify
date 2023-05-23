using NodifyOperations;
using System;

namespace NodifyOperations
{
    public class ValueOperation : IOperation
    {
        private readonly Func<double> _func;

        public ValueOperation(Func<double> func) => _func = func;

        public IOValue[] Execute(params IOValue[] operands) => new[] { new IOValue(default, _func())};
    }
}

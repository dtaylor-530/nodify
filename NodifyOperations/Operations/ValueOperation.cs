using System;

namespace Nodify.Demo
{
    public class ValueOperation : IOperation
    {
        private readonly Func<double> _func;

        public ValueOperation(Func<double> func) => _func = func;

        public object Execute(params object[] operands)
            => _func();
    }
}

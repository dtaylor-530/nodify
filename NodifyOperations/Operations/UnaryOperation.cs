using System;
using static Utility.Conversions.ConversionHelper;

namespace Nodify.Demo
{
    public class UnaryOperation : IOperation
    {
        private readonly Func<double, double> _func;

        public UnaryOperation(Func<double, double> func) => _func = func;

        public object Execute(params object[] operands)
            => _func.Invoke(ChangeType<double>(operands[0]));
    }
}

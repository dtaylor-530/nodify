using System;
using static Utility.Conversions.ConversionHelper;

namespace Nodify.Demo
{
    public class BinaryOperation : IOperation
    {
        private readonly Func<double, double, double> _func;

        public BinaryOperation(Func<double, double, double> func) => _func = func;

        public object Execute(params object[] operands)
            => _func.Invoke(ChangeType<double>(operands[0]), (double)ChangeType<double>(operands[1]));
    }
}

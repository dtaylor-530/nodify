using System;
using System.Linq;
using static Utility.Conversions.ConversionHelper;

namespace Nodify.Demo
{
    public class ParamsOperation : IOperation
    {
        private readonly Func<double[], double> _func;

        public ParamsOperation(Func<double[], double> func) => _func = func;

        public object Execute(params object[] operands)
            => _func.Invoke(operands.Select(a => ChangeType<double>(a)).ToArray());
    }
}

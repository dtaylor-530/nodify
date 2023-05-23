using Nodify.Operations;

namespace Nodify.Operations
{
    public interface IOperation
    {
        IOValue[] Execute(params IOValue[] operands);
    }

    public interface IFilter
    {
        bool Execute(object value);
    }
}

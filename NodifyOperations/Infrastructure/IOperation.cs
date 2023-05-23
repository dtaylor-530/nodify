using NodifyOperations;

namespace NodifyOperations
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

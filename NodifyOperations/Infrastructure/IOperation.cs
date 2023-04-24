namespace Nodify.Demo
{
    public interface IOperation
    {
        object Execute(params object[] operands);
    }

    public interface IFilter
    {
        bool Execute(object value);
    }
}

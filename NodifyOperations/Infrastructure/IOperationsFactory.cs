using NodifyOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodifyOperations.Infrastructure
{
    public interface IOperationsFactory
    {
        IEnumerable<OperationInfo> GetOperations();
    }
}

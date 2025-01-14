﻿using Nodify.Operations;
using Nodify.Operations.Infrastructure;
using System;
using System.Collections.Generic;
using static Utility.Conversions.ConversionHelper;

namespace Nodify.Demo.Infrastructure
{
    public class StandardOperationsFactory : IOperationsFactory
    {
        public IEnumerable<OperationInfo> GetOperations()
        {
            yield break;
            //yield return new OperationInfo
            //{
            //    Type = OperationType.Graph,
            //    Title = "(New) Operation Graph",
            //};
            //yield return new OperationInfo
            //{
            //    Type = OperationType.Calculator,
            //    Title = "Calculator"
            //};
            //yield return new OperationInfo
            //{
            //    Type = OperationType.Expression,
            //    Title = "Custom",
            //};
        }
    }

    public class MethodsOperationsFactory : IOperationsFactory
    {
        public IEnumerable<OperationInfo> GetOperations()
        {
            return OperationFactory.GetOperationsInfo(typeof(Methods));
        }

    }

    public class CustomOperationsFactory : IOperationsFactory
    {

        public const string Source = nameof(Source);
        public const string Target = nameof(Target);
        public const string Interface = nameof(Interface);

        public IEnumerable<OperationInfo> GetOperations()
        {
            yield return new OperationInfo
            {
                    Title = Source,
                    Type = OperationType.Normal,
                    Operation = new SourceOperation(),
                    MinInput = 1,
                    MaxInput = 1,
            };
            yield return new OperationInfo
            {
                Title = Target,
                Type = OperationType.Normal,
                Operation = new TargetOperation(),
                MinInput = 1,
                MaxInput = 1
            };
        }


        public class SourceOperation : IOperation
        {
            private readonly Func<bool, string> _func = a => a ? "Hello" : "Goodbye";

            public SourceOperation() { }

            public IOValue[] Execute(params IOValue[] operands)
                => new[] { new IOValue(default, _func.Invoke(ChangeType<bool>(operands[0].Value))) };
        }

        public class TargetOperation : IOperation
        {
            private readonly Func<string, bool> _func = a => a switch { "Hello" => true, "Goodbye" => false, _ => false };

            public TargetOperation() { }

            public IOValue[] Execute(params IOValue[] operands)
                => new[] { new IOValue(default, _func.Invoke(ChangeType<string>(operands[0].Value))) };
        }
    }
}


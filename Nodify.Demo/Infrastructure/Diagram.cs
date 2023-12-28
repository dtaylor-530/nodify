using DryIoc;
using Nodify.Abstractions;
using Nodify.Operations;
using System.Collections.Generic;
using System.Drawing;
using IContainer = DryIoc.IContainer;

namespace Nodify.Demo.Infrastructure
{
    using Node = Nodify.Abstractions.Node;
    using Connector = Nodify.Abstractions.Connector;
    using Connection = Nodify.Abstractions.Connection;

    public class Diagram1
    {

        public static Diagram Create()
        {
            //Name = "One";

            //var @interface = container.Resolve<OperationInterfaceNodeViewModel>();
            //var input3 = new Connector() { Title = "Input3" };
            //var input4 = new Connector() { Title = "Input4" };
            //var output3 = new Connector() { Title = "Output1", };
            //var output4 = new Connector() { Title = "Output2", };
            //@interface.Input.Add(input3);
            //@interface.Input.Add(input4);
            //@interface.Output.Add(output3);
            //@interface.Output.Add(output4);
            //@interface.Initialise();

            string connectionKey = "connection1";

            var source = new Node { Key = CustomOperationsFactory.Source };
            var input1 = new Connector() { Key = "Input1", Node = source.Key };
            var output1 = new Connector() { Key = "Output1", Node = source.Key, Connection = connectionKey };
            source.Input = new List<string> { input1.Key };
            source.Output = new List<string> { output1.Key };

            var target = new Node { Key = CustomOperationsFactory.Target };
            var input2 = new Connector() { Key = "Input2", Node= target.Key, Connection = connectionKey };
            var output2 = new Connector() { Key = "Output2", Node = target.Key };

            target.Input = new List<string> { input2.Key };
            target.Output = new List<string> { output2.Key };

            var connection = new Connection { Key = connectionKey, Output = output1.Key, Input = input2.Key };
            //var connection1 = new ConnectionViewModel { Output = output3, Input = input1 };
            //var connection2 = new ConnectionViewModel { Output = output2, Input = input4 };


            return new Diagram
            {
                Name = "Diagram 1",
                Nodes = new[] { source, target/*, @interface*/ },
                Connections = new[] { connection,/* connection1, connection2*/ },
                Connectors = new[] { input1, output1, input2, output2/* connection1, connection2*/ }
            };
        }

        //public override ConnectionViewModel[] Connections { get; }

        //public override NodeViewModel[] Nodes { get; }

        //public override ConnectorViewModel[] Connectors { get; }
    }
}

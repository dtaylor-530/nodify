using Nodify.Core;
using Nodify.Demo.Infrastructure;

namespace Nodify.Demo.ViewModels
{
    public class Diagram1 : Diagram
    {

        public Diagram1()
        {
            Name = "One";
            var output1 = new ConnectorViewModel() { Title = "Output1" };
            var source = new OperationNodeViewModel { Title = CustomOperationsFactory.Source, Output = output1, Location = new System.Windows.Point(100, 300) };
            var input1 = Globals.ViewModelInputConnector;
            source.Input.Add(input1);
            source.OnInputValueChanged();

            var output2 = Globals.ViewModelOutputConnector;
            var target = new OperationNodeViewModel { Title = CustomOperationsFactory.Target, Output = output2, Location = new System.Windows.Point(600, 300) };
            var input2 = new ConnectorViewModel() { Title = "Input2", Value = "Default" };
            target.Input.Add(input2);
            target.OnInputValueChanged();
            var connection = new OperationConnectionViewModel() { Input = input2, Output = output1 };
            Nodes = new[] { source, target };
            Connections = new[] { connection };
        }
        public override OperationConnectionViewModel[] Connections { get; }
        public override NodeViewModel[] Nodes { get; }
    }


}

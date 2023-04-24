using Autofac;
using Nodify.Core;
using System;
using System.Collections.Generic;

namespace Nodify.Demo.Infrastructure
{
    public class Globals
    {
        public static IContainer Container { get; set; }

        public static Dictionary<string, OperationInfo> Operations { get; set; }

        public static IEnumerable<Diagram> Diagrams { get; set; }

        public static Diagram SelectedDiagram { get; set; }

        public static ConnectorViewModel ViewModelInputConnector { get; set; } = new (){ Title="Input Connector"  };
        public static ConnectorViewModel ViewModelOutputConnector { get; set; } = new (){ Title="Output Connector"  };
    }
}

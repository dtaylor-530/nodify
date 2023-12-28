using System;

namespace Nodify.Abstractions
{
    public class Diagram
    {
        public Diagram() { }
        public string Name { get; set; }
        public Connection[] Connections { get; set; }
        public Node[] Nodes { get; set; }
        public Connector[] Connectors { get; set; }

        public static Diagram Empty => new Diagram();
    }
}

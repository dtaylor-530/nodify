namespace Nodify.Abstractions
{
    public class Connector: IDiagramObject
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public string Node { get; set; }
        public string Connection { get; set; }
    }
}

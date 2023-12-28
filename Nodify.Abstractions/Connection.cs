namespace Nodify.Abstractions
{
    public class Connection: IDiagramObject
    {
        public string Key { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }

    }
}

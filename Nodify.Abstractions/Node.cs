using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Abstractions
{

    public class Node: IDiagramObject
    {
        public string Key { get; set; }

        public List<string> Input { get; set; }
        public List<string> Output { get; set; }

    }
}

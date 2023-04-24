using Nodify.Core;
using Nodify.Demo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Demo
{
    public class DiagramsViewModel : ObservableObject
    {
        public IEnumerable<Diagram> Diagrams => Globals.Diagrams;

        public Diagram SelectedDiagram
        {
            get => Globals.SelectedDiagram;
            set
            {
                Globals.SelectedDiagram = value;
                this.OnPropertyChanged();
            }
        }
    }
}

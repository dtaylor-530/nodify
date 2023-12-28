//using Autofac;
using DryIoc;
using Nodify.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Nodify.Demo
{
    public class DiagramsViewModel : ObservableObject
    {
        private readonly IContainer container;

        public DiagramsViewModel(IContainer container)
        {
            this.container = container;
        }

        public IEnumerable<Diagram> Diagrams => container.Resolve<IEnumerable<Diagram>>(Keys.Diagrams);

        public Diagram SelectedDiagram
        {
            get => container.Resolve<NodifyObservableCollection<Diagram>>(Keys.SelectedDiagram).FirstOrDefault();
            set
            {
                container.Resolve<NodifyObservableCollection<Diagram>>(Keys.SelectedDiagram).Insert(0, value);
                this.OnPropertyChanged();
            }
        }
    }
}

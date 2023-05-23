using DryIoc;
using Nodify.Core;
using Nodify.Demo.ViewModels;
using System.Linq;

namespace Nodify.Demo
{
    public class MainViewModel
    {

        private readonly IContainer container;

        public MainViewModel(IContainer container)
        {
            this.container = container;
            TabsViewModel.AddEditorCommand.Execute(container.Resolve<DiagramsViewModel>());

            var selected = container.Resolve<NodifyObservableCollection<Diagram>>(serviceKey:Keys.SelectedDiagram);
            selected.CollectionChanged += MainViewModel_CollectionChanged;
            if (selected.FirstOrDefault() is Diagram item)
            {
                TabsViewModel.AddEditorCommand.Execute(container.Resolve<OperationsEditorViewModel>());
            }
        }

        private void MainViewModel_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //foreach (Diagram item in e.NewItems)
            //{
                TabsViewModel.AddEditorCommand.Execute(container.Resolve<OperationsEditorViewModel>());
           // }
        }

        public MessagesViewModel MessagesViewModel => container.Resolve<MessagesViewModel>();
        public TabsViewModel TabsViewModel => container.Resolve<TabsViewModel>();
        public InterfaceViewModel InterfaceViewModel => container.Resolve<InterfaceViewModel>();


    }
}

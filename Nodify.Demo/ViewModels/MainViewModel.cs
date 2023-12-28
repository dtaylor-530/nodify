using DryIoc;
using Nodify.Abstractions;
using Nodify.Core;
using Nodify.Extra;
using System;
using System.Linq;
using System.Windows.Input;

namespace Nodify.Demo
{
    public class MainViewModel
    {

        private readonly IResolverContext container;
        private readonly DelegateCommand reloadCommand;

        public MainViewModel(IResolverContext container)
        {
            this.container = container;
            TabsViewModel.AddEditorCommand.Execute(container.Resolve<DiagramsViewModel>());

            var selected = container.Resolve<NodifyObservableCollection<Diagram>>(serviceKey: Keys.SelectedDiagram);
            selected.CollectionChanged += MainViewModel_CollectionChanged;
            if (selected.FirstOrDefault() is Diagram item)
            {
                TabsViewModel.AddEditorCommand.Execute(container.Resolve<EditorViewModel>());
            }

            reloadCommand = new DelegateCommand(() =>
            {
                var diagram = container.Resolve<Diagram>();

                var connector = diagram.Connectors.Single(a => a.Key == InterfaceViewModel.Title);

                if (connector.Value != InterfaceViewModel.Value)
                {
                    connector.Value = InterfaceViewModel.Value;
                    container.Resolve<IObserver<IDiagramObject>>().OnNext(connector);
                }
            });
        }
        public ICommand ReloadCommand => reloadCommand;

        private void MainViewModel_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //foreach (Diagram item in e.NewItems)
            //{
            TabsViewModel.AddEditorCommand.Execute(container.Resolve<EditorViewModel>());
            // }
        }

        public MessagesViewModel MessagesViewModel => container.Resolve<MessagesViewModel>();
        public TabsViewModel TabsViewModel => container.Resolve<TabsViewModel>();
        public ConnectorViewModel InterfaceViewModel
        {
            get
            {
                var connector = container.Resolve<NodeViewModel>(Keys.Root).Input.First();
                return connector;
            }
        }
    }
}

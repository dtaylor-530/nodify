using Nodify.Core;
using System;
using System.ComponentModel;
using System.Linq;

namespace Nodify.Core
{

    public class NodeViewModel : BaseNodeViewModel
    {
        public event Action<NodeViewModel> InputChanged;

        private NodifyObservableCollection<ConnectorViewModel> input = new(), output = new();


        public NodeViewModel()
        {
            _ = Input.WhenAdded(x =>
            {
                x.Node = this;
                x.IsInput = true;
                x.PropertyChanged += OnInputValueChanged;

            })
            .WhenRemoved(x =>
            {
                x.PropertyChanged -= OnInputValueChanged;
            });
        }

        private void OnInputValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectorViewModel.Value))
            {
                OnInputValueChanged(sender as ConnectorViewModel);
            }
        }

        public Guid Id { get; } = Guid.NewGuid();
        public Key Key => new(Id, Title);



        public INodifyObservableCollection<ConnectorViewModel> Input => input;

        public INodifyObservableCollection<ConnectorViewModel> Output => output;

        public virtual void OnInputValueChanged(ConnectorViewModel connectorViewModel)
        {
        }
    }
}

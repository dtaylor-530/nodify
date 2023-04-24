using Nodify.Core;
using System;
using System.ComponentModel;
using System.Linq;

namespace Nodify.Core
{

    public class NodeViewModel : BaseNodeViewModel
    {
        public event Action<NodeViewModel> InputChanged;

        private ConnectorViewModel? _output;
        private NodifyObservableCollection<ConnectorViewModel> input { get; } = new();


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
                OnInputValueChanged();
            }
        }

        public Guid Id { get; } = Guid.NewGuid();
        public Key Key => new(Id, Title);



        public INodifyObservableCollection<ConnectorViewModel> Input => input;

        public ConnectorViewModel? Output
        {
            get => _output;
            set
            {
                if (SetProperty(ref _output, value) && _output != null)
                {
                    _output.Node = this;
                }
            }
        }

        public virtual async void OnInputValueChanged()
        {
        }
    }
}

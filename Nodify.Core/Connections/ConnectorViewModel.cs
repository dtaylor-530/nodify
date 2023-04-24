using System.Collections.Generic;
using System.Windows;

namespace Nodify.Core
{
    public class ConnectorViewModel : ObservableObject
    {
        private NodeViewModel _operation = default!;

        private string? _title;
        private object _value;
        private bool _isConnected;
        private bool _isInput;
        private Point _anchor;

        //public ConnectionViewModel()
        //{
        //    this.PropertyChanged += ConnectorViewModel_PropertyChanged;
        //}

        //private void ConnectorViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if(e.PropertyName==nameof(Value))
        //    {
        //        MessagesViewModel.OnNext(this);
        //    }
        //}

        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public object Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
                   //.Then(() => ValueObservers.ForEach(o => o.Value = value));

        }

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        public bool IsInput
        {
            get => _isInput;
            set => SetProperty(ref _isInput, value);
        }

        public Point Anchor
        {
            get => _anchor;
            set => SetProperty(ref _anchor, value);
        }


        public NodeViewModel Node
        {
            get => _operation;
            set => SetProperty(ref _operation, value);
        }

        //public List<ConnectorViewModel> ValueObservers { get; } = new List<ConnectorViewModel>();


    }
}

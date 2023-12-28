using Nodify.Core.Common;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Nodify.Core
{
    public enum ValueType
    {

    }

    public class ConnectorViewModel : BaseViewModel
    {
        private NodeViewModel _node = default!;


        private object _value;
        private bool _isConnected;
        private bool _isInput;
        private Point _anchor;
        private ValueType _valueType;


          public object Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public ValueType ValueType
        {
            get => _valueType;
            set => SetProperty(ref _valueType, value);
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
            get => _node;
            set => SetProperty(ref _node, value);
        }

    }
}

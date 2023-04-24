using System.Windows;

namespace Nodify.Core
{
    public class PendingConnectionViewModel : ObservableObject
    {
        private ConnectorViewModel _source = default!, _target;
        private bool _isVisible;
        private Point _targetLocation;

        public ConnectorViewModel Source
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }

        public ConnectorViewModel? Target
        {
            get => _target;
            set => SetProperty(ref _target, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public Point TargetLocation
        {
            get => _targetLocation;
            set => SetProperty(ref _targetLocation, value);
        }
    }
}

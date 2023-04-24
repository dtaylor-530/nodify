using System;
using System.Windows;

namespace Nodify.Core
{
    public class BaseNodeViewModel : ObservableObject
    {
        public event Action? Closed;

        private bool _isVisible; 
        private Point _location;
        private Size _size; 
        private string? _title;
        private bool _isActive;
        private bool _isSelected;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                SetProperty(ref _isVisible, value);
                if (!value)
                {
                    Closed?.Invoke();
                }
            }
        }

        public Point Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }


        public Size Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }


        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }


        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }


        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public bool IsReadOnly { get; set; }
    }
}

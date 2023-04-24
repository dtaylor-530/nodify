using System;
using Nodify.Demo.Infrastructure;
using System.Collections.Generic;
using Nodify.Core;

namespace Nodify.Demo
{
    public class InterfaceViewModel
    {
        BooleanViewModel booleanInViewModel = new ();
        ViewModel outViewModel = new ();
        public InterfaceViewModel()
        {  
            ViewModels = new ObservableObject[] { booleanInViewModel, outViewModel };
            booleanInViewModel.PropertyChanged += BooleanViewModel_PropertyChanged;
            Globals.ViewModelOutputConnector.PropertyChanged += ViewModelOutputConnector_PropertyChanged;
        }

        private void ViewModelOutputConnector_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName== nameof(ConnectorViewModel.Value))
            {
                outViewModel.Value = Globals.ViewModelOutputConnector.Value;
            }
        }

        private void BooleanViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Globals.ViewModelInputConnector.Value = booleanInViewModel.Value;
        }

        public ICollection<ObservableObject> ViewModels { get; }
    }


    public class BooleanViewModel : ObservableObject
    {
        private bool _value;

        public bool Value
        {
            get => _value;
            set => this.SetProperty(ref _value, value);
        }


    }

    public class ViewModel : ObservableObject
    {
        private object _value;

        public object Value
        {
            get => _value;
            set => this.SetProperty(ref _value, value);
        }


    }
}

using Nodify;
using Nodify.Core;
using System;

namespace NodifyOperations
{
    public class OperationConnectionViewModel : ConnectionViewModel
    {

        public static IObserver<ObservableObject> Observer;


        protected override void Output_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectorViewModel.Value))
            {
                Observer.OnNext(this);
            }
        }
    }
}

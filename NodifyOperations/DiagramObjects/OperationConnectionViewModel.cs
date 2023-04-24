using Nodify.Core;
using System;

namespace Nodify.Demo
{
    public class OperationConnectionViewModel : ConnectionViewModel
    {

        public static MessagesViewModel MessagesViewModel;


        protected override void Output_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName==nameof(ConnectorViewModel.Value))
            {
                MessagesViewModel.OnNext(this);
            }
        }
    }
}

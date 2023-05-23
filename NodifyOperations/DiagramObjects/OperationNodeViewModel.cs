using Nodify;
using Nodify.Core;
using System;
using System.ComponentModel;
using System.Linq;

namespace NodifyOperations
{
    public class OperationNodeViewModel : NodeViewModel
    {
        public static IObserver<ObservableObject> Observer;

        public OperationNodeViewModel()
        {

        }


        public override void OnInputValueChanged(ConnectorViewModel connectorViewModel)
        {

            Observer.OnNext(this);
        }
    }
}

using Nodify.Core;
using System;

namespace Nodify.Operations
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

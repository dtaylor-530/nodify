//using Autofac;
using DryIoc;
using Nodify.Core;
using Nodify.Demo.Infrastructure;
using Nodify.Operations;
using System;
using System.Linq;
using System.Reactive.Linq;
using IContainer = DryIoc.IContainer;

namespace Nodify.Demo.ViewModels
{
    public class OperationInterfaceNodeViewModel : OperationNodeViewModel
    {
        private readonly IContainer container;

        IObservable<PropertyChange> observable => container.Resolve<IObservable<PropertyChange>>(Keys.Pipe);

        public OperationInterfaceNodeViewModel(IContainer container)
        {
            Title = CustomOperationsFactory.Interface;
            Location = new System.Windows.Point(300, 100);
            this.container = container;
        }

        public void Initialise()
        {
            observable
                .Subscribe(a =>
                {
                    foreach (var input in Input.Where(a => a.Title == "Input3"))
                        input.Value = a;
                });
        }

        public override void OnInputValueChanged(ConnectorViewModel connectorViewModel)
        {
            if (connectorViewModel.Title == "Input4")
            {
                if (connectorViewModel.Value is var value)
                {
                    container.Resolve<ViewModel>().Value = value;
                }
            }
            base.OnInputValueChanged(connectorViewModel);
        }
    }
}

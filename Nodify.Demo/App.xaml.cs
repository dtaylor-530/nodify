//using Autofac;
using DryIoc;
using Nodify.Core;
using Nodify.Demo.Infrastructure;
using NodifyOperations;
using System;
using System.Windows;

namespace Nodify.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //var container = Bootstrapper.Build();
            var container = DryBootstrapper.Build();

            OperationNodeViewModel.Observer = container.Resolve<IObserver<ObservableObject>>();
            OperationConnectionViewModel.Observer = container.Resolve<IObserver<ObservableObject>>();

            DockWindow dockWindow = new()
            {
                DataContext = container.Resolve<MainViewModel>()
            };

            Window window = new()
            {
                Content = container.Resolve<MainViewModel>()
            };


            dockWindow.Show();
            window.Show();
        }

    }


}

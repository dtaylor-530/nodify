using Autofac;
using Nodify.Demo;
using Nodify.Demo.Infrastructure;
using Nodify.Demo.ViewModels;
using NodifyOperations.Infrastructure;
using System.Collections.Generic;
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

            var builder = new ContainerBuilder();

            builder.RegisterType<MethodsOperationsFactory>().As<IOperationsFactory>().SingleInstance();
            builder.RegisterType<StandardOperationsFactory>().As<IOperationsFactory>().SingleInstance();
            builder.RegisterType<CustomOperationsFactory>().As<IOperationsFactory>().SingleInstance();

            var x = new MessagesViewModel();
            OperationNodeViewModel.MessagesViewModel = x;
            OperationConnectionViewModel.MessagesViewModel = x;
            Globals.Container = builder.Build();
            Globals.Diagrams = new[] { new Diagram1() };

            Dictionary<string, OperationInfo> dictionary = new();

            foreach (var container in Globals.Container.Resolve<IEnumerable<IOperationsFactory>>())
            {
                foreach (var item in container.GetOperations())
                {
                    dictionary[item.Title] = item;
                }
            }

            Dictionary<string, FilterInfo> filters = new();



            Globals.Operations = dictionary;

            x.Operations = dictionary;
            x.Filters = filters;
            var main = new MainViewModel { MessagesViewModel = x };

            DockWindow dockWindow = new()
            {
                DataContext = main
            };
            
            Window window = new()
            {
                Content = main
            };

            
            dockWindow.Show();
            window.Show();
        }

    }
}

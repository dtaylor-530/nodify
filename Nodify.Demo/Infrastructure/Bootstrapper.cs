using Nodify.Core;
using Nodify.Operations.Infrastructure;
using Nodify.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using OperationKeys = Nodify.Operations.Keys;
using DemoKeys = Nodify.Demo.Keys;
using DryIoc;
using Nodify.Abstractions;
using Nodify.Extra;
using Nodify.Core.Common;

namespace Nodify.Demo.Infrastructure
{
    using Nodify.Abstractions;
    using System.Collections.ObjectModel;

    public class Bootstrapper
    {
        public static class DiConfiguration
        {
            public static Rules SetRules(Rules rules)
            {
                rules = rules
                    .WithDefaultReuse(Reuse.Singleton)
                    .With(FactoryMethod.ConstructorWithResolvableArguments);
                return rules;
            }

        }
        public static IContainer Build()
        {
            var builder = new Container(DiConfiguration.SetRules);

            RegisterOperations(builder);
            builder.Register<IOperationsFactory, MethodsOperationsFactory>();
            builder.Register<IOperationsFactory, CustomOperationsFactory>();
            builder.Register<IObserver<IDiagramObject>, Nodify.Operations.Resolver>();
            builder.RegisterDelegate(a =>
            {
                return Diagram1.Create();
            });
            builder.Register<DiagramsViewModel>();
            builder.Register<MainViewModel>();
            builder.Register<TabsViewModel>();
            builder.RegisterDelegate<DiagramViewModel>(a =>
            {
                var diagram = a.Resolve<Diagram>();
                var diagramViewModel = ViewModelConverter.Convert(diagram);
                return diagramViewModel;
            }, serviceKey:Keys.Diagram);      
            
            builder.RegisterDelegate<NodeViewModel>(a =>
            {
                var diagramViewModel = a.Resolve<DiagramViewModel>(Keys.Diagram);
    
                return diagramViewModel.Nodes.First();
            }, serviceKey:Keys.Root);

            builder.Register<MessagesViewModel>();
            //builder.Register<InterfaceViewModel>();
            builder.Register<EditorViewModel>();
            //builder.Register<ObservableObject, BooleanViewModel>();
            //builder.Register<ObservableObject, ViewModel>();
            builder.RegisterInstanceMany<ISubject<PropertyChange>>(new ReplaySubject<PropertyChange>(1), serviceKey: DemoKeys.Pipe);
            builder.RegisterInstanceMany<ISubject<object>>(new ReplaySubject<object>(1), serviceKey: OperationKeys.Next);
            builder.RegisterInstanceMany<ISubject<object>>(new ReplaySubject<object>(1), serviceKey: OperationKeys.Previous);
            builder.RegisterDelegate<ICommand>(c => new DelegateCommand(() => c.Resolve<IObserver<object>>(OperationKeys.Next).OnNext(default)), serviceKey: Keys.NextCommand);
            builder.RegisterDelegate<ICommand>(c => new DelegateCommand(() => c.Resolve<IObserver<object>>(OperationKeys.Previous).OnNext(default)), serviceKey: Keys.PreviousCommand);
            builder.RegisterMany<Dictionary<string, FilterInfo>>();
            builder.Register<NodifyObservableCollection<Diagram>>(serviceKey: DemoKeys.SelectedDiagram);
            builder.RegisterDelegate(c => c.Resolve<IEnumerable<Diagram>>(), serviceKey: DemoKeys.Diagrams);
            //builder.Register<OperationInterfaceNodeViewModel>();

            builder.RegisterDelegate<Dictionary<string, Node>>(a=> a.Resolve<Diagram>().Nodes.ToDictionary(a => a.Key, a => a),serviceKey: OperationKeys.Nodes);
            builder.RegisterDelegate<Dictionary<string, Connection>>(a => a.Resolve<Diagram>().Connections.ToDictionary(a => a.Key, a => a), serviceKey: OperationKeys.Connections);
            builder.RegisterDelegate<Dictionary<string, Connector>>(a => a.Resolve<Diagram>().Connectors.ToDictionary(a => a.Key, a => a), serviceKey: OperationKeys.Connectors);


            builder.RegisterMany<Dictionary<string, OperationInfo>>(serviceKey: OperationKeys.Operations);
            builder.RegisterDelegate<IDictionary<string, FilterInfo>>(()=> new Dictionary<string, FilterInfo>(),serviceKey: OperationKeys.Filters);
            builder.RegisterDelegate(c => c.Resolve<IDictionary<string, OperationInfo>>(OperationKeys.Operations).Keys.Select(a => new MenuItemViewModel() { Content = a }));
            builder.Register<ObservableCollection<Message>>(serviceKey: OperationKeys.Future);
            builder.Register<ObservableCollection<Message>>(serviceKey: OperationKeys.Current);
            builder.Register<ObservableCollection<Message>>(serviceKey: OperationKeys.Past);

            builder.RegisterInitializer<ObservableObject>((a, context) =>
            {
                a.PropertyChanges().Subscribe(oo =>
                {
                    context
                      .Resolve<IObserver<PropertyChange>>(DemoKeys.Pipe)
                      .OnNext(oo);
                });
            });

            return builder;
        }


        private static void RegisterOperations(Container builder)
        {
            builder.RegisterDelegate<IDictionary<string, OperationInfo>>(a =>
            {
                Dictionary<string, OperationInfo> dictionary = new();

                foreach (var container in a.Resolve<IEnumerable<IOperationsFactory>>())
                {
                    foreach (var item in container.GetOperations())
                    {
                        dictionary[item.Title] = item;
                    }
                }

                return dictionary;
            });
        }
    }

}

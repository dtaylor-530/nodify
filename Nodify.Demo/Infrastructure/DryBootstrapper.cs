
using Nodify.Core;
using Nodify.Demo.ViewModels;
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

namespace Nodify.Demo.Infrastructure
{
    public class DryBootstrapper
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
            builder.Register<IObserver<ObservableObject>, Nodify.Operations.Resolver>();
            builder.Register<Diagram, Diagram1>();
            builder.Register<DiagramsViewModel>();
            builder.Register<MainViewModel>();
            builder.Register<TabsViewModel>();
            builder.Register<MessagesViewModel>();
            builder.Register<InterfaceViewModel>();
            builder.Register<OperationsEditorViewModel>();
            builder.Register<ObservableObject, BooleanViewModel>();
            builder.Register<ObservableObject, ViewModel>();
            builder.RegisterInstanceMany<ISubject<PropertyChange>>(new ReplaySubject<PropertyChange>(1), serviceKey: DemoKeys.Pipe);
            builder.RegisterInstanceMany<ISubject<object>>(new ReplaySubject<object>(1), serviceKey: OperationKeys.Next);
            builder.RegisterInstanceMany<ISubject<object>>(new ReplaySubject<object>(1), serviceKey: OperationKeys.Previous);
            builder.RegisterDelegate<ICommand>(c => new DelegateCommand(() => c.Resolve<IObserver<object>>(OperationKeys.Next).OnNext(default)), serviceKey: Keys.NextCommand);
            builder.RegisterDelegate<ICommand>(c => new DelegateCommand(() => c.Resolve<IObserver<object>>(OperationKeys.Previous).OnNext(default)), serviceKey: Keys.PreviousCommand);
            builder.RegisterMany<Dictionary<string, FilterInfo>>();
            builder.Register<NodifyObservableCollection<Diagram>>(serviceKey: DemoKeys.SelectedDiagram);
            builder.RegisterDelegate(c => c.Resolve<IEnumerable<Diagram>>(), serviceKey: DemoKeys.Diagrams);
            builder.Register<OperationInterfaceNodeViewModel>();
            builder.RegisterMany<Dictionary<Core.Key, NodeViewModel>>(serviceKey: OperationKeys.Nodes);
            builder.RegisterMany<Dictionary<Core.Key, ConnectionViewModel>>(serviceKey: OperationKeys.Connections);
            builder.RegisterMany<Dictionary<string, OperationInfo>>(serviceKey: OperationKeys.Operations);
            builder.RegisterDelegate<IDictionary<string, FilterInfo>>(()=> new Dictionary<string, FilterInfo>(),serviceKey: OperationKeys.Filters);
            builder.RegisterDelegate(c => c.Resolve<IDictionary<string, OperationInfo>>(OperationKeys.Operations).Keys.Select(a => new MenuItemViewModel() { Content = a }));
            builder.Register<NodifyObservableCollection<Message>>(serviceKey: OperationKeys.Future);
            builder.Register<NodifyObservableCollection<Message>>(serviceKey: OperationKeys.Current);
            builder.Register<NodifyObservableCollection<Message>>(serviceKey: OperationKeys.Past);

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

                dictionary["Interface"] = new OperationInfo
                {
                    Title = "Interface",
                    Type = OperationType.Normal,
                    Operation = new Operation2(),
                    MinInput = 1,
                    MaxInput = 1
                };

                return dictionary;
            });
        }
    }

    public class Operation2 : IOperation
    {
        public IOValue[] Execute(params IOValue[] operands)
        {
            if (operands.Single(a => a.Title == "Input3").Value is PropertyChange { Source: BooleanViewModel { Guid: var guid, Value :bool value } })
            {
                if(guid == Guids.Boolean)
                {
                    return new[] { new IOValue( "Output1", value )/*, new IOValue { Title="Output2", Value = default} */};
                }
            }
            return new IOValue[0];
        }
    }
}

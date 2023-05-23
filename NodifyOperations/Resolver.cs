﻿using Nodify.Core;
using NodifyOperations;
using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
//using Autofac;
using DryIoc;
using Nodify;

namespace NodifyOperations
{
    public class Resolver : ObservableObject, IObserver<ObservableObject>
    {
        SynchronizationContext context = SynchronizationContext.Current ?? throw new Exception("dfs 3!!!");

        private readonly IContainer container;
        private IObservable<object> next => container.Resolve<IObservable<object>>(Keys.Next);
        private IObservable<object> previous => container.Resolve<IObservable<object>>(Keys.Previous);
        private ICollection<Exception> exceptions => container.Resolve<ICollection<Exception>>(Keys.Exceptions);
        private IList<Message> past => container.Resolve<NodifyObservableCollection<Message>>(Keys.Past);
        private IList<Message> current => container.Resolve<NodifyObservableCollection<Message>>(Keys.Current);
        private IList<Message> future => container.Resolve<NodifyObservableCollection<Message>>(Keys.Future);
        private IDictionary<Key, NodeViewModel> nodes => container.Resolve<IDictionary<Key, NodeViewModel>>(Keys.Nodes);
        private IDictionary<Key, ConnectionViewModel> connections => container.Resolve<IDictionary<Key, ConnectionViewModel>>(Keys.Connections);
        private IDictionary<string, OperationInfo> operations => container.Resolve<IDictionary<string, OperationInfo>>(Keys.Operations);
        private IDictionary<string, FilterInfo> filters => container.Resolve<IDictionary<string, FilterInfo>>(Keys.Filters);

        public Resolver(IContainer container)
        {
            this.container = container;
            next
                .Subscribe(async a =>
                {
                    var next = future[0];
                    future.RemoveAt(0);
                    current.Add(next);
                    if (current.Last() is NodeMessage nodeMessage)
                    {
                        var node = nodes[next.Key];
                        if (node.Output.Any())
                        {
                            Exception? ex = null;
                            IOValue[]? output = null;
                            
                            try
                            {
                                output = operations[next.Key.Name].Operation.Execute(nodeMessage.Inputs);

                            }
                            catch (Exception e)
                            {
                                ex = e;
                            }
                            //});
                            context.Post((a) =>
                            {
                                past.Add(next);
                                if(current.Remove(next)==false)
                                {
                                    throw new Exception("v222d sdww");
                                }
                                if (ex != null)
                                {
                                    next = next with { Exception = ex };
                                    exceptions.Add(ex);
                                }
                                else if (output != null)
                                {
                                    foreach (var connector in node.Output)
                                    {
                                        foreach (var outp in output)
                                            if (outp.Title == connector.Title || outp.Title == default)
                                                connector.Value = outp.Value;
                                    }
                                }
                                else
                                {
                                    throw new Exception("d11 fs 3??l!!!");
                                }
                            }, default);

                        }
                        else
                        {
                            if (current.Remove(next) == false)
                            {
                                throw new Exception("vd sdww");
                            }
                        }
                    }
                    else if (current.Last() is ConnectionMessage connectorMessage)
                    {
                        var connection = connections[next.Key];
                        if (connection.Input != null)
                        {
                            try
                            {
                                past.Add(next);
                                if (current.Remove(next) == false)
                                {
                                    throw new Exception("v222d sdww");
                                }
                                if (filters.ContainsKey(next.Key.Name))
                                {
                                    if (filters[next.Key.Name].Filter.Execute(connectorMessage.Output))
                                        connection.Input.Value = connectorMessage.Output;
                                }
                                else
                                {
                                    connection.Input.Value = connectorMessage.Output;
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    else
                    {
                        throw new Exception("143 34vd sdww");
                    }
                });

            //previous = new DelegateCommand(() =>
            //{
            //    var previous = Past.Last();
            //    Future.Insert(0, previous);
            //    Future.RemoveAt(0);
            //});
            this.container = container;
        }


        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(ObservableObject @object)
        {
            if (@object is NodeViewModel node)
            {
                var values = node.Input.Select(a => new IOValue(a.Title, a.Value)).ToArray();
                nodes[node.Key] = node;
                future.Add(new NodeMessage(node.Key, values));
            }
            else if (@object is OperationConnectionViewModel connection)
            {
                connections[connection.Key] = connection;
                future.Add(new ConnectionMessage(connection.Key, connection.Output.Value));
            }
        }
    }

    public record Message(Key Key, Exception Exception);

    public record NodeMessage(Key Key, IOValue[] Inputs, Exception Exception = default) : Message(Key, default);


    public record ConnectionMessage(Key Key, object Output) : Message(Key, default);


    public record IOValue(string Title, object Value);
}


using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DryIoc;
using System.Collections.ObjectModel;
using Nodify.Abstractions;

namespace Nodify.Operations
{
    public class Resolver : IObserver<IDiagramObject>
    {
        //SynchronizationContext context = SynchronizationContext.Current ?? throw new Exception("dfs 3!!!");

        private readonly IContainer container;
        private IObservable<object> next => container.Resolve<IObservable<object>>(Keys.Next);
        private IObservable<object> previous => container.Resolve<IObservable<object>>(Keys.Previous);
        private ICollection<Exception> exceptions => container.Resolve<ICollection<Exception>>(Keys.Exceptions);
        private IList<Message> past => container.Resolve<ObservableCollection<Message>>(Keys.Past);
        private IList<Message> current => container.Resolve<ObservableCollection<Message>>(Keys.Current);
        private IList<Message> future => container.Resolve<ObservableCollection<Message>>(Keys.Future);
        private IDictionary<string, Node> nodes => container.Resolve<Dictionary<string, Node>>(Keys.Nodes);
        private IDictionary<string, Connection> connections => container.Resolve<Dictionary<string, Connection>>(Keys.Connections);
        private IDictionary<string, Connector> connectors => container.Resolve<Dictionary<string, Connector>>(Keys.Connectors);

        private IDictionary<string, OperationInfo> operations => container.Resolve<IDictionary<string, OperationInfo>>(Keys.Operations);
        private IDictionary<string, FilterInfo> filters => container.Resolve<IDictionary<string, FilterInfo>>(Keys.Filters);

        public Resolver(IContainer container)
        {
            this.container = container;
            next
                .Subscribe(async a =>
                {
                    var _next = future[0];
                    future.RemoveAt(0);
                    current.Add(_next);
                    if (current.Last() is NodeMessage nodeMessage)
                    {
                        var node = nodes[_next.Key];
                        if (node.Output.Any())
                        {
                            Exception? ex = null;
                            IOValue[]? output = null;

                            try
                            {
                                output = operations[_next.Key].Operation.Execute(nodeMessage.Inputs);

                            }
                            catch (Exception e)
                            {
                                ex = e;
                            }
                            //});
                            //context.Post((a) =>
                            //{
                            past.Add(_next);
                            if (current.Remove(_next) == false)
                            {
                                throw new Exception("v222d sdww");
                            }
                            if (ex != null)
                            {
                                _next = _next with { Exception = ex };
                                exceptions.Add(ex);
                            }
                            else if (output != null)
                            {
                                foreach (var _out in node.Output)
                                {
                                    var connector = connectors[_out];
                                    foreach (var outp in output)
                                        if (outp.Title == connector.Key || outp.Title == default)
                                        {
                                            connector.Value = outp.Value;
                                            this.OnNext(connector);
                                        }
                                }
                            }
                            else
                            {
                                throw new Exception("d11 fs 3??l!!!");
                            }
                            //}, default);

                        }
                        else
                        {
                            if (current.Remove(_next) == false)
                            {
                                throw new Exception("vd sdww");
                            }
                        }
                    }
                    else if (current.Last() is ConnectionMessage connectionMessage)
                    {
                        var connection = connections[_next.Key];
                        if (connection.Input != null)
                        {

                            past.Add(_next);
                            if (current.Remove(_next) == false)
                            {
                                throw new Exception("v222d sdww");
                            }
                            if (filters.ContainsKey(_next.Key))
                            {
                                if (filters[_next.Key].Filter.Execute(connectionMessage.Output))
                                    connectors[connection.Input].Value = connectionMessage.Output;
                            }
                            else
                            {
                                connectors[connection.Input].Value = connectionMessage.Output;
                            }
                            this.OnNext(connectors[connection.Input]);

                        }
                    }
                    else if (current.Last() is ConnectorMessage connectorMessage)
                    {
                        var connector = connectors[_next.Key];
                        if (nodes[connector.Node].Input.Contains(connector.Key))
                        {

                            past.Add(_next);
                            if (current.Remove(_next) == false)
                            {
                                throw new Exception("v222d sdww");
                            }
                            this.OnNext(nodes[connector.Node]);  
                        }
                        else if (nodes[connector.Node].Output.Contains(connector.Key))
                        {
                            past.Add(_next);
                            if (current.Remove(_next) == false)
                            {
                                throw new Exception("v222d sdww");
                            }
                            if (connector.Connection != null)
                                this.OnNext(connections[connector.Connection]);

                        }
                    }
                    else
                    {
                        throw new Exception("143 34vd sdww");
                    }
                });

 
            this.container = container;
        }


        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(IDiagramObject @object)
        {
            if (@object is Node node)
            {
                var values = node.Input.Select(a => new IOValue(a, connectors[a].Value)).ToArray();
                //nodes[node.Key] = node;
                future.Add(new NodeMessage(node.Key, values));
            }
            else if (@object is Connection connection)
            {
                //connections[connection.Key] = connection;
                future.Add(new ConnectionMessage(connection.Key, connectors[connection.Output].Value));
            }
            else if (@object is Connector connector)
            {
                //connectors[connector.Key] = connector;
                future.Add(new ConnectorMessage(connector.Key, connector.Value));
            }
        }
    }

    public record Message(string Key, Exception Exception);

    public record NodeMessage(string Key, IOValue[] Inputs, Exception Exception = default) : Message(Key, default);


    public record ConnectionMessage(string Key, object Output) : Message(Key, default);
    public record ConnectorMessage(string Title, object Output) : Message(Title, default);


    public record IOValue(string Title, object Value);
}

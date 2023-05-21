using Nodify.Core;
using Nodify.Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using ICommand = System.Windows.Input.ICommand;
using System.Xml.Linq;

namespace Nodify.Demo
{
    public class MessagesViewModel : ObservableObject, IObserver<ObservableObject>
    {
        private NodifyObservableCollection<Message> _past = new(), _future = new();
        Dictionary<Key, NodeViewModel> nodes = new();
        Dictionary<Key, OperationConnectionViewModel> connections = new();
        public Dictionary<string, OperationInfo> Operations { get; set; }
        public Dictionary<string, FilterInfo> Filters { get; set; }
        public MessagesViewModel()
        {
            Next = new DelegateCommand(() =>
            {
                var next = Future[0];
                if (next is NodeMessage nodeMessage)
                {
                    var node = nodes[next.Key];
                    if (node.Output != null)
                    {
                        try
                        {
                            foreach(var output in node.Output)
                            {
                                output.Value = Operations[next.Key.Name].Operation.Execute(nodeMessage.Inputs);
                            }
          
                        }
                        catch(Exception ex)
                        {

                        }
                    }
                }
                else if(next is ConnectionMessage connectorMessage)
                {
                    var connection = connections[next.Key];
                    if (connection.Input != null)
                    {
                        try
                        {
                            if (Filters.ContainsKey(next.Key.Name))
                            {
                                if (Filters[next.Key.Name].Filter.Execute(connectorMessage.Output))
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

                Past.Add(next);
                Future.RemoveAt(0);
            });

            Previous = new DelegateCommand(() =>
            {
                var previous = Past.Last();
                Future.Insert(0, previous);
                Future.RemoveAt(0);
            });
        }


        public ICommand Next { get; }
        public ICommand Previous { get; }

        public NodifyObservableCollection<Message> Past
        {
            get => _past;
        }

        public NodifyObservableCollection<Message> Future
        {
            get => _future;
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
                var values = node.Input.Select(a => a.Value).ToArray();
                nodes[node.Key] = node;
                Future.Add(new NodeMessage { Inputs = values, Key = node.Key });
            }
            else if (@object is OperationConnectionViewModel connection)
            {
                //var values = connector.V
                connections[connection.Key] = connection;
                Future.Add(new ConnectionMessage { Output = connection.Output.Value, Key = connection.Key });
            }
        }
    }

    public class Message
    {
        public Key Source { get; set; }
        public Key Target { get; set; }
        public Key Key { get; set; }
    }


    public class NodeMessage : Message
    {
        public object[] Inputs { get; set; }
    }

    public class ConnectionMessage : Message
    {
        public object Output { get; set; }

    }
}

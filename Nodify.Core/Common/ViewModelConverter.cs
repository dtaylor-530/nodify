using Nodify.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Core.Common
{
    public class ViewModelConverter
    {


        public static DiagramViewModel Convert(Diagram diagram)
        {
            var viewModel = new DiagramViewModel();

            Dictionary<string, ConnectorViewModel> keyValues = new Dictionary<string, ConnectorViewModel>();

            foreach (var node in diagram.Nodes)
            {
                var nodeViewModel = Convert(node);
                var x = node.Input.Select(a => new ConnectorViewModel { Title = a, IsInput = true }).ToArray();
                var y = node.Output.Select(a => new ConnectorViewModel { Title = a }).ToArray();
                foreach (var item in x)
                    keyValues.Add(item.Title, item);  
                
                foreach (var item in y)
                    keyValues.Add(item.Title,  item);

                nodeViewModel.Input.AddRange(x);
                nodeViewModel.Output.AddRange(y);
                viewModel.Nodes.Add(nodeViewModel);
            }

            foreach (var connection in diagram.Connections)
            {
                viewModel.Connections.Add(new ConnectionViewModel
                {
                    Title = connection.Key,
                    Input = keyValues[connection.Input],
                    Output = keyValues[connection.Output]
                });
            }


            return viewModel;
        }

        public static NodeViewModel Convert(Node node)
        {
            return new NodeViewModel
            {
                Title = node.Key
            };
        }


        //public ConnectionViewModel Convert(Connection node)
        //{
        //    return new ConnectionViewModel
        //    {
        //        Title = node.Key,

        //    };
        //}
    }
}

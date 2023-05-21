using Nodify.Core;
using Nodify.Demo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nodify.Demo.ViewModels
{
    public class OperationsEditorViewModel : EditorViewModel
    {
        public OperationsEditorViewModel(Diagram diagram) : base(diagram)
        {
        }

        protected override void OperationsMenu_Selected(Point location, MenuItemViewModel menuItem)
        {
            NodeViewModel op = OperationFactory.CreateNode(Globals.Operations[menuItem.Content]);
            op.Location = location;

            Nodes.Add(op);

            var pending = PendingConnection;
            if (pending.IsVisible)
            {
                var connector = pending.Source.IsInput ? op.Output.FirstOrDefault() : op.Input.FirstOrDefault();
                if (connector != null && CanCreateConnection(pending.Source, connector))
                {
                    CreateConnection(pending.Source, connector);
                }
            }
        }

        protected override IEnumerable<MenuItemViewModel> MenuItems()
        {
            return Globals.Operations.Keys.Select(a => new MenuItemViewModel() { Content = a });
        }

        protected override void CreateConnection(ConnectorViewModel source, ConnectorViewModel? target)
        {
            if (target == null)
            {
                PendingConnection.IsVisible = true;
                Menu.OpenAt(PendingConnection.TargetLocation);
                Menu.Closed += OnOperationsMenuClosed;
                return;
            }

            var input = source.IsInput ? source : target;
            var output = target.IsInput ? source : target;

            PendingConnection.IsVisible = false;

            DisconnectConnector(input);

            Connections.Add(new OperationConnectionViewModel
            {
                Input = input,
                Output = output
            });
        }
    }
}

//using Autofac;
using DryIoc;
using Nodify.Abstractions;
using Nodify.Core;
using Nodify.Core.Common;
using Nodify.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Nodify.Demo
{
    public class EditorViewModel
    {
        private readonly IResolverContext container;
        private MenuViewModel menu;
        private DiagramViewModel diagramViewModel => container.Resolve<DiagramViewModel>(Keys.Diagram);
        private IDictionary<string, OperationInfo> operations => container.Resolve<IDictionary<string, OperationInfo>>(Keys.Operations);

        public EditorViewModel(IResolverContext container) : base()
        {
            this.container = container;

        }

        protected IEnumerable<MenuItemViewModel> MenuItems => container.Resolve<IEnumerable<MenuItemViewModel>>();

        void OperationsMenu_Selected(Point location, MenuItemViewModel menuItem)
        {
            NodeViewModel op = ViewModelConverter.Convert(OperationFactory.CreateNode(operations[menuItem.Content]));
            op.Location = location;

            DiagramViewModel.Nodes.Add(op);

            var pending = DiagramViewModel.PendingConnection;
            if (pending.IsVisible)
            {
                var connector = pending.Source.IsInput ? op.Output.FirstOrDefault() : op.Input.FirstOrDefault();
                //if (connector != null && CanCreateConnection(pending.Source, connector))
                //{
                //    CreateConnection(pending.Source, connector);
                //}
            }
        }

        public DiagramViewModel DiagramViewModel
        {
            get
            {
 
                diagramViewModel.ConnectionCreated -= DiagramViewModel_ConnectionCreated;
                diagramViewModel.ConnectionCreated += DiagramViewModel_ConnectionCreated;
                return diagramViewModel;
            }
        }

        private void DiagramViewModel_ConnectionCreated((ConnectorViewModel source, ConnectorViewModel? target) obj)
        {
            if (obj.target == null)
            {
                DiagramViewModel.PendingConnection.IsVisible = true;
                Menu.OpenAt(DiagramViewModel.PendingConnection.TargetLocation);
                Menu.Closed += OnOperationsMenuClosed;
            }

        }

        public MenuViewModel Menu
        {
            get
            {
                if (menu == null)
                {
                    menu = new MenuViewModel();
                    menu.Selected += OperationsMenu_Selected;
                    menu.Items.AddRange(MenuItems);

                }
                return menu;
            }
        }



        protected void OnOperationsMenuClosed()
        {
            DiagramViewModel.PendingConnection.IsVisible = false;
            Menu.Closed -= OnOperationsMenuClosed;
        }

    }
}

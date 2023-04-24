using Nodify.Core;
using Nodify.Demo.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Demo.ViewModels
{
    public class OperationTabsViewModel : TabsViewModel
    {
        protected override object Content { get => new OperationsEditorViewModel(Diagram.Empty);  }
    }
}

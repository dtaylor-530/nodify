using Nodify.Core;
using System;
using System.ComponentModel;
using System.Linq;

namespace Nodify.Demo
{
    public class OperationNodeViewModel : NodeViewModel
    {
        public static MessagesViewModel MessagesViewModel;

        public OperationNodeViewModel()
        {

        }


        public override void OnInputValueChanged()
        {

            MessagesViewModel.OnNext(this);
        }
    }
}

using Nodify.Core;
using System;
using System.Collections.Generic;
using ICommand = System.Windows.Input.ICommand;
using DryIoc;
using Nodify.Operations;
using System.Collections.ObjectModel;

namespace Nodify.Demo
{
    public class MessagesViewModel : ObservableObject
    {
        private readonly IContainer container;

        public MessagesViewModel(IContainer container)
        {
            this.container = container;
        }


        public ICommand Next => container.Resolve<ICommand>(Keys.NextCommand);

        public ICommand Previous => container.Resolve<ICommand>(Keys.PreviousCommand);

        public ICollection<Message> Current => container.Resolve<ObservableCollection<Message>>(Keys.Current);

        public ICollection<Message> Past => container.Resolve<ObservableCollection<Message>>(Keys.Past);

        public ICollection<Message> Future => container.Resolve<ObservableCollection<Message>>(Keys.Future);

        public ICollection<Exception> Exceptions => container.Resolve<ObservableCollection<Exception>>(Keys.Exceptions);
     
    }
}

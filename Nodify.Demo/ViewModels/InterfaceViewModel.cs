using System;
using Nodify.Demo.Infrastructure;
using System.Collections.Generic;
using Nodify.Core;
using DryIoc;
//using Autofac;

namespace Nodify.Demo
{
    public class InterfaceViewModel
    {
        private readonly IContainer container;

        public InterfaceViewModel(IContainer container)
        {
            this.container = container;
        }

        public ICollection<ObservableObject> ViewModels => container.Resolve<ICollection<ObservableObject>>();
    }


    public class BooleanViewModel : ObservableObject
    {
        public Guid Guid => Guids.Boolean;

        private bool _value;

        public bool Value
        {
            get => _value;
            set => this.SetProperty(ref _value, value);
        }
    }

    public class ViewModel : ObservableObject
    {
        public Guid Guid => Guids.Default;

        private object _value;

        public object Value
        {
            get => _value;
            set => this.SetProperty(ref _value, value);
        }
    }
}

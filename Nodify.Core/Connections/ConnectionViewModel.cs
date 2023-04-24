using Nodify.Core;
using System;

namespace Nodify.Core
{
    public class ConnectionViewModel : ObservableObject
    {
        private ConnectorViewModel _output = default!, _input = default!;
        private string _title = string.Empty;


        public Guid Id { get; } = Guid.NewGuid();


        public Key Key => new(Id, string.Empty);


        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ConnectorViewModel Input
        {
            get => _input;
            set => SetProperty(ref _input, value);
        }

        public ConnectorViewModel Output
        {
            get => _output;
            set => SetProperty(ref _output, value)
                .Then(() =>
                {
                    this.Output.PropertyChanged += Output_PropertyChanged;
                });
        }

        protected virtual void Output_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }
    }
}

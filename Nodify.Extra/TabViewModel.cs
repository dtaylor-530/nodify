namespace Nodify.Extra
{
    public class TabViewModel : ObservableObject
    {
        public event Action<TabViewModel, object>? OnOpenInnerCalculator;
        public event Action<TabViewModel>? Close;

        public TabViewModel? Parent { get; set; }

        public TabViewModel()
        {
     
            OpenCalculatorCommand = new DelegateCommand<object>(calculator =>
            {
                OnOpenInnerCalculator?.Invoke(this, calculator);
            });   

            CloseCommand = new DelegateCommand<object>(a =>
            {
                Close?.Invoke(this);
            });
        }

        public INodifyCommand OpenCalculatorCommand { get; }
        public INodifyCommand CloseCommand { get; }

        public Guid Id { get; } = Guid.NewGuid();

        private object _calculator = default!;
        public object Content 
        {
            get => _calculator;
            set => SetProperty(ref _calculator, value);
        }

        private string? _name;
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }
}

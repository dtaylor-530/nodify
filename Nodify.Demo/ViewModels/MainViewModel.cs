using Nodify.Core;
using Nodify.Demo.Infrastructure;
using Nodify.Demo.ViewModels;

namespace Nodify.Demo
{
    public class MainViewModel
    {
        DiagramsViewModel DiagramsViewModel = new DiagramsViewModel();

        public MainViewModel()
        {
            //TabsViewModel.AddEditorCommand.Execute(default);
            TabsViewModel.AddEditorCommand.Execute(DiagramsViewModel);
            DiagramsViewModel.PropertyChanged += DiagramsViewModel_PropertyChanged;
        }

        private void DiagramsViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DiagramsViewModel.SelectedDiagram))
                TabsViewModel.AddEditorCommand.Execute(new OperationsEditorViewModel(DiagramsViewModel.SelectedDiagram));
        }

        public MessagesViewModel MessagesViewModel { get; set; }
        public TabsViewModel TabsViewModel { get; } = new OperationTabsViewModel();
        public InterfaceViewModel InterfaceViewModel { get; } = new InterfaceViewModel();


    }
}

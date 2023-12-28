using System;
using System.Windows.Input;

namespace Nodify.Core
{


    public class MenuItemViewModel
    {
        public event Action<MenuItemViewModel> Selected;

        public MenuItemViewModel()
        {
            Command = new DelegateCommand(() =>
            {
                Selected?.Invoke(this);
            });
        }

        public string? Content { get; set; }

        public ICommand Command { get; set; }

    }
}

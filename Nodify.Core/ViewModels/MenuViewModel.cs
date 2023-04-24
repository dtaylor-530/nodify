using System;
using System.Windows;

namespace Nodify.Core
{
    public class MenuViewModel : BaseNodeViewModel
    {
        public event Action<Point, MenuItemViewModel> Selected;

        public MenuViewModel()
        {
            (Items = new NodifyObservableCollection<MenuItemViewModel>())
                .WhenAdded(a =>
            {
                a.Selected += selected;
            });
        }

        private void selected(MenuItemViewModel obj)
        {
            Selected?.Invoke(Location, obj);
            Close();
        }

        public NodifyObservableCollection<MenuItemViewModel> Items { get; }


        public void OpenAt(Point targetLocation)
        {
            Close();
            Location = targetLocation;
            IsVisible = true;
        }

        public void Close()
        {
            IsVisible = false;
        }
    }
}

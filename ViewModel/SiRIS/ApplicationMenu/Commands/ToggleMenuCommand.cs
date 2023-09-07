using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.Commands
{
    public class ToggleMenuCommand : ICommand
    {
        public ApplicationMenuViewModel ViewModel { get; set; }
        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public ToggleMenuCommand(ApplicationMenuViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (ViewModel.IsVisible == System.Windows.Visibility.Visible)
            {
                ViewModel.CollapseButtonImage = "ChevronDown";
                ViewModel.IsVisible = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ViewModel.CollapseButtonImage = "ChevronUp";
                ViewModel.IsVisible = System.Windows.Visibility.Visible;
            }
        }
    }
}

using SiRISApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.Commands
{
    public class DisplayApplicationCommand : ICommand
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

        public DisplayApplicationCommand(ApplicationMenuViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return ViewModel.IsVisible == System.Windows.Visibility.Visible;
        }

        public void Execute(object? parameter)
        {
            if (parameter != null)
                AppExecutionService.Instance.OpenProcess((string)parameter);
        }
    }
}

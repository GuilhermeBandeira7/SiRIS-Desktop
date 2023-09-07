using SiRISApp.ViewModel.SessionPlayer.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SessionPlayer.Commands
{
    public class UpdateSourceCommand : ICommand
    {

        SourceViewModel _viewModel;

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

        public UpdateSourceCommand(SourceViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _viewModel.UpdateSourceEvent?.Invoke(this, new UpdateSourceEventArg() { Source = _viewModel.ObsSource } );
        }
    }
}

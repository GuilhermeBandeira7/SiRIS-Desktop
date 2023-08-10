using SiRISApp.ViewModel.SessionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SessionManagement.Commads
{
    public class EditSessionCommand : ICommand
    {
        private readonly SessionViewModel _vm;

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

        public EditSessionCommand(SessionViewModel vm)
        {
            _vm = vm;
        }


        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _vm.EditSession();
        }
    }
}

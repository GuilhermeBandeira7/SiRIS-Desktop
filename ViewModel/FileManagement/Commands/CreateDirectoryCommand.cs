using EntityMtwServer.Services.SiRIS;
using SiRISApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.FileManagement.Commands
{
    public class CreateDirectoryCommand : ICommand
    {
        FileManagementViewModel fileManagementViewModel;
        
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

        public CreateDirectoryCommand(FileManagementViewModel fileManagementViewModel)
        {
            this.fileManagementViewModel = fileManagementViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            fileManagementViewModel.CreateDirectory();
        }
    }
}

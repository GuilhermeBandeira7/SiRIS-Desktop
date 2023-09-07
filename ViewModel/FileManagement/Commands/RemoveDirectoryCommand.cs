using SiRISApp.Services;
using SiRISApp.ViewModel.FileManagement.Folder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.FileManagement.Commands
{
    public class RemoveDirectoryCommand : ICommand
    {
        public FileManagementViewModel ViewModel { get; set; }     

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

        public RemoveDirectoryCommand(FileManagementViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            if(parameter == null) 
                return false;

            try
            {
                FolderViewModel folderViewModel = (FolderViewModel)parameter;
                return folderViewModel.Path != string.Empty && folderViewModel.Path != AppSessionService.Instance.User.Registration;
            }
            catch
            {
                return false;
            }
        }

        public void Execute(object? parameter)
        {
            ViewModel.RemoveDirectory();
        }
    }
}

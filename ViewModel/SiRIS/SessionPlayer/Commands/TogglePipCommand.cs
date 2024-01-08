using SiRISApp.Services;
using SiRISApp.ViewModel.SessionPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SiRISApp.ViewModel.SiRIS.SessionPlayer
{
    public class TogglePipCommand : ICommand
    {
        SessionPlayerViewModel ViewModel { get; set; }
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

        public TogglePipCommand(SessionPlayerViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (!OBSService.Instance.isPipActive)
            {
                if (OBSService.Instance.CurrentSource != "Camera Principal")
                {
                    ViewModel.EnableCameraText = "disableCamera";
                    OBSService.Instance.ActivatePip();
                }
                else
                {
                    MessageService.Instance.Show("error", "A câmera ja está ativada como fonte principal de transmissão, por favor desativa a câmera para poder usar essa opção!");
                }
            }
            else
            {
                OBSService.Instance.DesactivatePip();
                ViewModel.EnableCameraText = "enableCamera";
            }
              
        }
    }
}

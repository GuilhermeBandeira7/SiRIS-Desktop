using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SiRISApp.ViewModel.SiRIS
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string message = string.Empty;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private string primaryColor = "#132c21";
        public string PrimaryColor
        {
            get { return primaryColor; }
            set
            {
                primaryColor = value;
                OnPropertyChanged(nameof(PrimaryColor));
            }
        }

        private string secondaryColor = "#132c21";
        public string SecondaryColor
        {
            get { return secondaryColor; }
            set
            {
                secondaryColor = value;
                OnPropertyChanged(nameof(SecondaryColor));
            }
        }


        public void SetMessage(string type, string message)
        {
            if (type == "error")
            {
                PrimaryColor = "#6c1321";
                SecondaryColor = "#4c1321";
            }
            else if (type == "warning")
            {
                PrimaryColor = "#8c6c00";
                SecondaryColor = "#6c4c00";
            }
            Message = message;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

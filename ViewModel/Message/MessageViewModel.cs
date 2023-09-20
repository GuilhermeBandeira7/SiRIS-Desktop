using Newtonsoft.Json.Linq;
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

        private string primaryColor = "#36B285";
        public string PrimaryColor
        {
            get { return primaryColor; }
            set
            {
                primaryColor = value;
                OnPropertyChanged(nameof(PrimaryColor));
            }
        }

        private string secondaryColor = "#42886E";
        public string SecondaryColor
        {
            get { return secondaryColor; }
            set
            {
                secondaryColor = value;
                OnPropertyChanged(nameof(SecondaryColor));
            }
        }

        private string image = "pack://application:,,,/View/Assets/LottieAnimations/success.json";
        public string Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged(nameof(Image));

            }
        }

        private int progressBarValue;
        public int ProgressBarValue
        {
            get { return progressBarValue; }
            set
            {
                progressBarValue = value;
                OnPropertyChanged(nameof(ProgressBarValue));
            }
        }

        public void SetMessage(string type, string message)
        {
            if (type == "error")
            {
                PrimaryColor = "#E84C3D";
                SecondaryColor = "#EC6E60";
                Image = "pack://application:,,,/View/Assets/LottieAnimations/error.json";
            }
            else if (type == "warning")
            {
                PrimaryColor = "#FFA400";
                SecondaryColor = "#FFD38A";
                Image = "pack://application:,,,/View/Assets/LottieAnimations/warning.json";
            }

            var app = (App)System.Windows.Application.Current;
            string? text = app.LanguageDictionary[message]?.ToString();

            if(text != null) Message = text;
            else Message = message;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

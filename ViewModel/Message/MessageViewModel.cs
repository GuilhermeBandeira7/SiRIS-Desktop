using Humanizer;
using Newtonsoft.Json.Linq;
using SiRISApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SiRISApp.ViewModel.SiRIS
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string type = string.Empty;
        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged(nameof(Type));

            }
        }

        private string text = string.Empty;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
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

        private int statusBarValue = 0;
        public int StatusBarValue
        {
            get { return statusBarValue; }
            set
            {
                statusBarValue = value;
                OnPropertyChanged(nameof(StatusBarValue));
            }
        }

        private int maximum = 100;
        public int Maximum
        {
            get { return maximum; }
            set
            {
                maximum = value;
                OnPropertyChanged(nameof(Maximum));

            }
        }

        private int minimum = 0;
        public int Minimum
        {
            get { return minimum; }
            set
            {
                minimum = value;
                OnPropertyChanged(nameof(Minimum));
            }
        }

        private int position = 0;
        public int Position
        {
            get { return position; }
            set
            {
                position = value;
                OnPropertyChanged(nameof(position));

            }
        }

        private double progressBarValue = 0;
        public double ProgressBarValue
        {
            get { return progressBarValue; }
            set
            {
                progressBarValue = value;
                OnPropertyChanged(nameof(ProgressBarValue));
            }
        }

        private string progressText = "0/0";
        public string ProgressText
        {
            get { return progressText; }
            set
            {
                progressText = value;
                OnPropertyChanged(nameof(ProgressText));

            }
        }

        private Visibility statusBarVisibility;
        public Visibility StatusBarVisibility
        {
            get { return statusBarVisibility; }
            set
            {
                statusBarVisibility = value;
                OnPropertyChanged(nameof(StatusBarVisibility));
            }
        }

        private Visibility inputVisibility;
        public Visibility InputVisibility
        {
            get { return inputVisibility; }
            set
            {
                inputVisibility = value;
                OnPropertyChanged(nameof(InputVisibility));

            }
        }

        private Visibility progressBarVisibility;
        public Visibility ProgressBarVisibility
        {
            get { return progressBarVisibility; }
            set
            {
                progressBarVisibility = value;
                OnPropertyChanged(nameof(ProgressBarVisibility));

            }
        }

        private bool buttons = false;
        private bool loading = false;

        public MessageViewModel()
        {

        }

        public MessageViewModel(MessageModel message)
        {
            Init(message);
        }

        public void Init(MessageModel message)
        {
            buttons = message.Buttons;
            loading = message.Loading;
            Minimum = message.Min;
            Maximum = message.Max;
            Type = message.Type;

            StatusBarVisibility = buttons || loading ? Visibility.Collapsed : Visibility.Visible;
            InputVisibility = buttons ? Visibility.Visible : Visibility.Collapsed;
            ProgressBarVisibility = loading ? Visibility.Visible : Visibility.Collapsed;

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
            else if (type == "info")
            {
                PrimaryColor = "#739DE1";
                SecondaryColor = "#2355A6";
                Image = $"pack://application:,,,/View/Assets/LottieAnimations/{message.Image}.json";
            }

            App app = (App)Application.Current;
            string? text = app.LanguageDictionary[message.Text]?.ToString();

            if (text != null) Text = text;
            else Text = message.Text;
        }

        public void StepProgressBar()
        {
            if (loading)
            {
                double stepValue = 100 / (double)(Maximum - Minimum);
                ProgressBarValue += stepValue;
                Position += 1;
                ProgressText = $"{Position}/{Maximum}";
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}

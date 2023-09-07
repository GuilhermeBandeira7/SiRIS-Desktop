using SiRISApp.ViewModel.SessionPlayer.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiRISApp.ViewModel.SessionPlayer
{
    public class SourceViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string obsSource = string.Empty;
        public string ObsSource
        {
            get { return obsSource; }
            set { obsSource = value; }
        }

        private string image = string.Empty;
        public string Image
        {
            get { return image; }
            set { image = value; }
        }

        private string color = string.Empty;
        public string Color
        {
            get { return color; }
            set { color = value; }
        }

        private string borderBrush = string.Empty;
        public string BorderBrush
        {
            get { return borderBrush; }
            set { borderBrush = value; }
        }

        private string foreColor = string.Empty;
        public string ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }

        public UpdateSourceCommand UpdateSourceCommand { get; set; }
        public EventHandler? UpdateSourceEvent { get; set; }

        public SourceViewModel(string image, string source, string color, string borderBrush, string foreColor)
        {
            UpdateSourceCommand = new UpdateSourceCommand(this);
            ObsSource = source;
            Image = image;
            Color = color;
            BorderBrush = borderBrush;
            ForeColor = foreColor;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}

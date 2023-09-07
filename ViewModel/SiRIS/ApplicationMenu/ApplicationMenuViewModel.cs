using SiRISApp.ViewModel.SiRIS.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SiRISApp.ViewModel.SiRIS
{
    public class ApplicationMenuViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Visibility isVisible = Visibility.Collapsed;
        public Visibility IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }


        private string collapseButtonImage = "ChevronDown";
        public string CollapseButtonImage
        {
            get { return collapseButtonImage; }
            set
            {
                collapseButtonImage = value;
                OnPropertyChanged(nameof(CollapseButtonImage));

            }
        }

        private string backgroundColor;
        public string BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }


        public ToggleMenuCommand ToggleMenuCommand { get; set; }
        public DisplayApplicationCommand DisplayApplicationCommand { get; set; }

        public ApplicationMenuViewModel()
        {
            ToggleMenuCommand = new(this);
            DisplayApplicationCommand = new(this);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

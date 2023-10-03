using SiRISApp.Services;
using SiRISApp.ViewModel.SessionPlayer.Commands;
using SiRISApp.ViewModel.SessionPlayer.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SiRISApp.ViewModel.SessionPlayer
{
    public class SessionPreviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool pipActive;
        public bool PipActive
        {
            get { return pipActive; }
            set
            {
                pipActive = value;
                OnPropertyChanged("PipActive");
            }
        }

        private BitmapImage? pip1Image;
        public BitmapImage? Pip1Image
        {
            get { return pip1Image; }
            set
            {
                pip1Image = value;
                OnPropertyChanged("Pip1Image");
            }
        }

        private string selectedSource = "Captura De Tela";
        public string SelectedSource
        {
            get { return selectedSource; }
            set
            {
                selectedSource = value;
                OnPropertyChanged(nameof(SelectedSource));
            }
        }

        private bool run = true;

        public ObservableCollection<SourceViewModel> Sources { get; set; } = new()
        {
            new("MonitorShare", "Captura De Tela", "#af04B8B4","DarkGray", "White" ),
            new("Webcam", "Camera Principal", "Gray", "DarkGray", "White"),
            new("MicrosoftExcel", "Excel", "#185C37", "DarkGray", "White"),
            new("MicrosoftPowerpoint", "PPTx", "#D35230", "DarkGray", "White"),
            new("FilePdfBox", "PDF", "Snow", "DarkGray", "Red"),
            new("MicrosoftWord", "Word", "#103F91", "DarkGray", "White"),
            new("FolderPlay", "Video Via Servidor", "#af282725", "DarkGray", "#FFCD41"),
            new("Filmstrip", "Video Via Local", "#af282725", "DarkGray", "#904DEB"),
            new("FolderImage", "Fotos Via Servidor", "#af282725", "DarkGray", "#2B73C2"),

        };

        public ActivateSourceCommand ActivateSourceCommand { get; set; }
 

        public SessionPreviewViewModel()
        {
            ActivateSourceCommand = new(this);

            foreach (SourceViewModel source in Sources)
                source.UpdateSourceEvent += UpdateSource;

            Task.Run(StartPip);
        }


        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StartPip()
        {
            while (run)
            {
                Thread.Sleep(100);
                try
                {
                    if (OBSService.Instance.State != OBS_STATE.DISCONNECTED && OBSService.Instance.State != OBS_STATE.CONNECTING && OBSService.Instance.State != OBS_STATE.STOPPED)
                    {
                        using (MemoryStream memory = OBSService.Instance.GetSceneStream(SelectedSource))
                        {
                            memory.Position = 0;
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = memory;
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.EndInit();
                            bitmapImage.Freeze();

                            Pip1Image = bitmapImage;
                        }
                    }

                }
                catch
                {

                }
            }
        }


        public void StopPip()
        {
            run = false;
        }

        private void UpdateSource(object? sender, EventArgs e)
        {
            UpdateSourceEventArg source = (UpdateSourceEventArg)e;
            if (source != null)
            {
                OBSService.Instance.EnableSource(source.Source);
                SelectedSource = source.Source;
            }
        }

    }
}

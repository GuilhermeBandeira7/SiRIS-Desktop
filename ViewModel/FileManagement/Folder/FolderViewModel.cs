using SiRISApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiRISApp.ViewModel.FileManagement.Folder
{
    public class FolderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));

            }
        }

        private string path = string.Empty;
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                OnPropertyChanged(nameof(Path));

            }
        }


        public ObservableCollection<FolderViewModel> SubFolders { get; set; } = new();
        public ObservableCollection<FileViewModel> Files { get; set; } = new();

        public FolderViewModel()
        {

        }

        public FolderViewModel(string name, string path)
        {
            Name = name;
            Path = path;

            List<string> f = FtpService.Instance.GetFolders(Path);
            foreach (string subfolder in f)
                SubFolders.Add(new(subfolder.Replace($"{name}/", ""), $"{path}/{subfolder.Split("/").Last()}"));

            foreach (string file in FtpService.Instance.GetFiles(Path))
                Files.Add(new(file.Replace($"{name}/", ""), $"{path}/{ file.Replace($"{name}/", "") }"));
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

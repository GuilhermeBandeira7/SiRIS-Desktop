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

        public ObservableCollection<FolderViewModel> FilteredSubFolders { get; set; } = new();
        public ObservableCollection<FileViewModel> FilteredFiles { get; set; } = new();

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

            ApplyFileFilter(string.Empty);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ApplyFolderFilter(string filter)
        {
            FilteredSubFolders.Clear();
            foreach(var subfolder in SubFolders)
            {
                subfolder.ApplyFolderFilter(filter);

                if (subfolder.Name.Contains(filter))
                    FilteredSubFolders.Add(subfolder);
                else if(subfolder.FilteredSubFolders.Count > 0)
                    FilteredSubFolders.Add(subfolder);

                  
            }
        }

        public void ApplyFileFilter(string filter)
        {
            FilteredFiles.Clear();
            foreach (var file in Files)
                if (file.Name.Contains(filter))
                    FilteredFiles.Add(file);
        }

        public void UnselectFiles()
        {
            foreach(var file in Files)
                file.IsSelected = false;
        }
    }
}

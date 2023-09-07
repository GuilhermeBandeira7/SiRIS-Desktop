using SiRISApp.Services;
using SiRISApp.ViewModel.FileManagement.Commands;
using SiRISApp.ViewModel.FileManagement.Folder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SiRISApp.ViewModel.FileManagement
{
    public class FileManagementViewModel : INotifyPropertyChanged
    {

        private string newDirectoryName = string.Empty;
        public string NewDirectoryName
        {
            get { return newDirectoryName; }
            set
            {
                newDirectoryName = value;
                OnPropertyChanged(nameof(NewDirectoryName));
            }
        }

        private string currentPath = AppSessionService.Instance.User.Registration;
        public string CurrentPath
        {
            get { return currentPath; }
            set
            {
                currentPath = value;
                OnPropertyChanged(nameof(CurrentPath));
            }
        }

        private FolderViewModel selectedFolder = new();
        public FolderViewModel SelectedFolder
        {
            get { return selectedFolder; }
            set
            {
                selectedFolder = value;
                if (value == null)
                    CurrentPath = AppSessionService.Instance.User.Registration;
                else
                    CurrentPath = value.Path;
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }

        private string selectedFile = string.Empty;
        public string SelectedFile
        {
            get { return selectedFile; }
            set
            {
                selectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
            }
        }

        public ObservableCollection<FolderViewModel> Folders { get; set; } = new();
        public ObservableCollection<FileViewModel> Files { get; set; } = new();

        public CreateDirectoryCommand CreateDirectoryCommand { get; set; }
        public RemoveDirectoryCommand RemoveDirectoryCommand { get; set; }
        public AddFilesCommand AddFileCommand { get; set; }
        public RemoveFilesCommand RemoveFilesCommand { get; set; }
        public DownloadFilesCommand DownloadFilesCommand { get; set; }
        public RunFilesCommands RunFilesCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public FileManagementViewModel()
        {
            CreateDirectoryCommand = new(this);
            RemoveDirectoryCommand = new(this);
            AddFileCommand = new(this);
            RemoveFilesCommand = new(this);
            DownloadFilesCommand = new(this);
            RunFilesCommand = new(this);
            LoadFolders();
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LoadFolders()
        {
            Folders.Clear();
            List<string> f = FtpService.Instance.GetFolders(string.Empty);
            foreach (string folder in f)
                Folders.Add(new(folder, folder));

            NewDirectoryName = string.Empty;
            CurrentPath = AppSessionService.Instance.User.Registration;
        }

        public void CreateDirectory()
        {
            string newFolderPath = CurrentPath == "/" ? $"{SelectedFolder.Path}{NewDirectoryName}" : $"{SelectedFolder.Path}/{NewDirectoryName}";
            FtpService.Instance.CreateFolder(newFolderPath);
            LoadFolders();
        }

        public void RemoveDirectory()
        {
            FtpService.Instance.DeleteFolder(CurrentPath);
            LoadFolders();
        }

        public void AddFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in openFileDialog.FileNames)
                    FtpService.Instance.UploadFile(currentPath, filename);
            }

            LoadFolders();
        }

        public void RunFiles()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string downloadDirectory = $"{currentDirectory}\\Downloads";
            if (!Directory.Exists(downloadDirectory))
                Directory.CreateDirectory(downloadDirectory);

            foreach (FileViewModel file in SelectedFolder.Files)
                FtpService.Instance.DownloadFile(file.Path, downloadDirectory);

            if (SelectedFolder.Files.Where(f => f.IsSelected).ToList().Count > 1)
                AppExecutionService.Instance.ExecuteListOfFiles(SelectedFolder.Files.Where(f => f.IsSelected).Select(f => $"{downloadDirectory}\\{f.Path.Split("/").Last()}").ToList());
            else
                AppExecutionService.Instance.ExecuteFile($"{downloadDirectory}\\{SelectedFolder.Files.Where(f => f.IsSelected).ToList()[0].Path.Split("/").Last()}" );

        }

        public void DownloadFiles()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                    foreach (FileViewModel file in SelectedFolder.Files.Where(f => f.IsSelected))
                        FtpService.Instance.DownloadFile(file.Path, dialog.SelectedPath);
            }
        }

        public void DeleteFiles()
        {
            foreach (FileViewModel file in SelectedFolder.Files.Where(f => f.IsSelected))
                FtpService.Instance.DeleteFile(file.Path);
            LoadFolders();
        }
    }
}

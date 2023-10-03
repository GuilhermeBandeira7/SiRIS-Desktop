using EntityMtwServer.Entities;
using ExtendedXmlSerializer;
using SiRISApp.Services;
using SiRISApp.ViewModel.FileManagement.Commands;
using SiRISApp.ViewModel.FileManagement.Folder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        private bool isSelecting = false;
        public bool IsSelecting
        {
            get { return isSelecting; }
            set
            {
                isSelecting = value;
                OnPropertyChanged(nameof(IsSelecting));
                SelectingVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                ManagingVisibility = !value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private string folderFilter = string.Empty;
        public string FolderFilter
        {
            get { return folderFilter; }
            set
            {


                folderFilter = value;
                if (value == null)
                    folderFilter = string.Empty;
                OnPropertyChanged(nameof(FolderFilter));
                FilteredFolders.Clear();
                foreach (var folder in Folders)
                {
                    folder.ApplyFolderFilter(folderFilter);
                    if (folder.Name.Contains(folderFilter))
                        FilteredFolders.Add(folder);
                    else if (folder.FilteredSubFolders.Count > 0)
                        FilteredFolders.Add(folder);
                }
            }
        }

        private string fileFilter = string.Empty;
        public string FileFilter
        {
            get { return fileFilter; }
            set
            {
                fileFilter = value;
                if (value == null)
                    fileFilter = string.Empty;

                OnPropertyChanged(nameof(FileFilter));
                SelectedFolder.ApplyFileFilter(fileFilter);
            }
        }

        private Visibility managingVisibility = Visibility.Visible;
        public Visibility ManagingVisibility
        {
            get
            {
                return managingVisibility;
            }
            set
            {
                managingVisibility = value;
                OnPropertyChanged(nameof(ManagingVisibility));
            }
        }

        private Visibility selectingVisibility = Visibility.Collapsed;
        public Visibility SelectingVisibility
        {
            get { return selectingVisibility; }
            set
            {
                selectingVisibility = value;
                OnPropertyChanged(nameof(SelectingVisibility));
            }
        }

        private FolderViewModel selectedFolder = new();
        public List<string> SelectedFiles { get; set; } = new();
        public FolderViewModel SelectedFolder
        {
            get { return selectedFolder; }
            set
            {
                if (value != null)
                {
                    selectedFolder = value;
                    SelectedFiles.RemoveAll(f => f.Contains(selectedFolder.Path));
                    foreach (FileViewModel file in selectedFolder.Files.Where(x => x.IsSelected))
                        SelectedFiles.Add(file.Path);

                    selectedFolder = value;
                    if (value == null)
                        CurrentPath = AppSessionService.Instance.User.Registration;
                    else
                        CurrentPath = value.Path;
                    OnPropertyChanged(nameof(SelectedFolder));
                }

            }
        }

        public ObservableCollection<string> Servers { get; set; } = new();
        private string selectedServer = string.Empty;
        public string SelectedServer
        {
            get { return selectedServer; }
            set
            {
                selectedServer = value;
                OnPropertyChanged(nameof(selectedServer));
                string serverIp = ServerConfigService.Instance.GetServers().Where(s => s.Value == value).Select(s => s.Key).First();
                FtpService.Instance.SetRoot(serverIp);
                LoadFolders();
            }
        }


        public ObservableCollection<FolderViewModel> Folders { get; set; } = new();
        public ObservableCollection<FolderViewModel> FilteredFolders { get; set; } = new();

        public CreateDirectoryCommand CreateDirectoryCommand { get; set; }
        public RemoveDirectoryCommand RemoveDirectoryCommand { get; set; }
        public AddFilesCommand AddFileCommand { get; set; }
        public RemoveFilesCommand RemoveFilesCommand { get; set; }
        public DownloadFilesCommand DownloadFilesCommand { get; set; }
        public RunFilesCommand RunFilesCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public FileManagementViewModel()
        {
            CreateDirectoryCommand = new(this);
            RemoveDirectoryCommand = new(this);
            AddFileCommand = new(this);
            RemoveFilesCommand = new(this);
            DownloadFilesCommand = new(this);
            RunFilesCommand = new(this);
            MessageService.Instance.Show("info", "loadingServers", false, true, 0, 1, "loadingServer");
        }

        public void Init()
        {
            Dictionary<string, string> availableServer = ServerConfigService.Instance.GetServers();


            foreach (var server in availableServer)
                Servers.Add(server.Value);
            MessageService.Instance.Step();
            Thread.Sleep(1000);

            ServerConfig serverConfig = ServerConfigService.Instance.GetServerConfig();
            SelectedServer = availableServer[serverConfig.Ip];
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LoadFolders()
        {
            MessageService.Instance.Show("info", "loadingFolders", false, true, 0, 2, "loadingFolder");
            Folders.Clear();
            List<string> f = FtpService.Instance.GetFolders(string.Empty);
            MessageService.Instance.Step();

            foreach (string folder in f)
                Folders.Add(new(folder, folder));

    
            NewDirectoryName = string.Empty;
            CurrentPath = AppSessionService.Instance.User.Registration;
            FolderFilter = string.Empty;
            SelectedFolder = Folders.First();
            MessageService.Instance.Step();
      
        }

        public void CreateDirectory()
        {
            string newFolderPath = CurrentPath == "/" ? $"{SelectedFolder.Path}{NewDirectoryName}" : $"{SelectedFolder.Path}/{NewDirectoryName}";
            Response response = FtpService.Instance.CreateFolder(newFolderPath);
            MessageService.Instance.ShowDialog(response.Result ? "sucess" : "error", response.Message);
            LoadFolders();
        }

        public void RemoveDirectory()
        {
            Response response = FtpService.Instance.DeleteFolder(CurrentPath);
            MessageService.Instance.ShowDialog(response.Result ? "sucess" : "error", response.Message);
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
                Response response = new(true, "uploadSuccess");
                MessageService.Instance.Show("info", "uploadFiles", false, true, 0, openFileDialog.FileNames.Count(), "uploadFiles");
                foreach (string filename in openFileDialog.FileNames)
                {
                    Response r = FtpService.Instance.UploadFile(currentPath, filename);
                    if (!r.Result)
                        response = r;

                    MessageService.Instance.Step();
                }

                MessageService.Instance.ShowDialog(response.Result ? "sucess" : "error", response.Message);
            }

     
            LoadFolders();
        }

        public void RunFiles()
        {
            AddSelectedFiles();
            string currentDirectory = Directory.GetCurrentDirectory();
            string downloadDirectory = $"{currentDirectory}\\Downloads";
            if (!Directory.Exists(downloadDirectory))
                Directory.CreateDirectory(downloadDirectory);

            Response response = new(true, "downloadSuccess");
            MessageService.Instance.Show("info", "downloadFiles", false, true, 0, SelectedFiles.Count, "downloadFiles");
            foreach (string file in SelectedFiles)
            {
        
                Response r = FtpService.Instance.DownloadFile(file, downloadDirectory);
                if (!r.Result)
                    response = r;
                MessageService.Instance.Step();
            }

            if (response.Result)
            {
                if (SelectedFolder.Files.Where(f => f.IsSelected).ToList().Count > 1)
                    response = AppExecutionService.Instance.ExecuteListOfFiles(SelectedFolder.Files.Where(f => f.IsSelected).Select(f => $"{downloadDirectory}\\{f.Path.Split("/").Last()}").ToList());
                else
                    response = AppExecutionService.Instance.ExecuteFile($"{downloadDirectory}\\{SelectedFolder.Files.Where(f => f.IsSelected).ToList()[0].Path.Split("/").Last()}");
            }

            if(!response.Result)
                MessageService.Instance.Show("error", response.Message);

            SelectedFiles.Clear();
            SelectedFolder.UnselectFiles();
        }

        public void DownloadFiles()
        {
            AddSelectedFiles();
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Response response = new(true, "downloadSuccess");
                    MessageService.Instance.Show("info", "downloadFiles", false, true, 0, SelectedFiles.Count, "downloadFiles");
                    foreach (string file in SelectedFiles)
                    {
                        MessageService.Instance.Step();
                        Response r = FtpService.Instance.DownloadFile(file, dialog.SelectedPath);
                        if (!r.Result)
                            response = r;
                    }

                    MessageService.Instance.Show(response.Result ? "sucess" : "error", response.Message);
                }
         
            }
        }

        public void DeleteFiles()
        {
            AddSelectedFiles();
            MessageService.Instance.Show("info", "deletingFiles", false, true, 0, SelectedFiles.Count, "deleteFiles");
            Response response = new(true, "deleteSuccess");
            foreach (string file in SelectedFiles)
            {
                MessageService.Instance.Step();
                Response r = FtpService.Instance.DeleteFile(file);
                if (!r.Result)
                    response = r;
            }

            MessageService.Instance.ShowDialog(response.Result ? "sucess" : "error", response.Message); 
            LoadFolders();
           
        }

        public void AddSelectedFiles()
        {
            if (SelectedFolder != null)
            {
                foreach (string file in SelectedFolder.Files.Where(f => f.IsSelected).Select(f => f.Path))
                    if (!SelectedFiles.Contains(file))
                        SelectedFiles.Add(file);
            }

        }
    }
}

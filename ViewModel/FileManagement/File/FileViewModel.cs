using System.ComponentModel;
using System.Linq;

namespace SiRISApp.ViewModel.FileManagement
{
    public class FileViewModel : INotifyPropertyChanged
    {

        private long id;
        public long Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

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

        public string image = string.Empty;
        public string Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                OnPropertyChanged(nameof(image));
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

        private string extension = string.Empty;
        public string Extension
        {
            get { return extension; }
            set
            {
                extension = value;
                OnPropertyChanged(nameof(extension));
            }
        }

        private bool isSelected = false;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public FileViewModel(string name, string path)
        {
            Path = path;
            Name = name;
            Extension = name.Split('.').Last();
            if (extension == "mkv" || extension == "mp4" || extension == "avi")
                Image = "Filmstrip";
            else if (extension == "pptx" || extension == "ppt")
                Image = "MicrosoftPowerpoint";
            else if (extension == "docx" || extension == "doc")
                Image = "MicrosoftWord";
            else if (extension == "xlsx" || extension == "xls")
                Image = "MicrosoftExcel";
            else if (extension == "pdf")
                Image = "FilePdfBox";
            else if (extension == "jpeg" || extension == "jpg" || extension == "png" || extension == "bnp")
                Image = "Image";
            else
                Image = "File";

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

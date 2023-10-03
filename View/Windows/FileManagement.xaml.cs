using SiRISApp.ViewModel.FileManagement;
using SiRISApp.ViewModel.FileManagement.Folder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SiRISApp.View.Windows
{
    /// <summary>
    /// Interaction logic for FileManagement.xaml
    /// </summary>
    public partial class FileManagement : Window
    {
        private bool isSelecting = false;
        public bool IsSelecting
        {
            get 
            { 
                return isSelecting;
            }
            set
            {
                isSelecting = value;
                if (Resources["vm"] != null)
                {
                    FileManagementViewModel viewModel = (FileManagementViewModel)Resources["vm"];
                    viewModel.IsSelecting = value;
                }
            }
        }

        public event EventHandler? ReturnSelectedFile = null;


        public FileManagement()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Resources["vm"] != null)
            {
                FileManagementViewModel viewModel = (FileManagementViewModel)Resources["vm"];
                viewModel.SelectedFolder = (FolderViewModel)e.NewValue;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Resources["vm"] != null)
            {
                FileManagementViewModel viewModel = (FileManagementViewModel)Resources["vm"];
                ReturnSelectedFileEventArg eventArg = new();
                viewModel.AddSelectedFiles();
                foreach (string file in viewModel.SelectedFiles) 
                    eventArg.SelectedFiles.Add(file);


                ReturnSelectedFile?.Invoke(this, eventArg);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                CreateServerCloseDialog.Command.Execute(null);
                CreateServerCommand.Command.Execute(null); ;
            }
     
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
            {
               
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (Resources["vm"] != null)
            {
                FileManagementViewModel viewModel = (FileManagementViewModel)Resources["vm"];
                viewModel.Init();
            }
        }
    }
}

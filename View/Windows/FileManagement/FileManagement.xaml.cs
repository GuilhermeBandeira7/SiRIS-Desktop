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
    }
}

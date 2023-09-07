using SiRISApp.View.Windows;
using SiRISApp.ViewModel;
using SiRISApp.ViewModel.SessionManagement;
using System.Windows.Controls;
using System.Windows.Input;

namespace SiRISApp.View.UserControls.SessionManagement
{
    /// <summary>
    /// Interaction logic for SessionManagement.xaml
    /// </summary>
    public partial class SessionManagement : UserControl
    {
        public SessionManagement()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            RefreshButton.Command.Execute(null);
        }

        private void FolderButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FileManagement fileManagement = new FileManagement();
            fileManagement.ShowDialog();    
        }
    }
}

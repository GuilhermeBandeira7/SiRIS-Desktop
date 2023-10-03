using Microsoft.Office.Interop.Word;
using SiRISApp.View.Windows;
using System.Windows.Controls;

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
            FileManagement fileManagement = new();
            fileManagement.ShowDialog();    
        }
    }
}

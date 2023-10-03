using SiRISApp.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace SiRISApp.View.UserControls.SessionManagement
{
    /// <summary>
    /// Interaction logic for Session.xaml
    /// </summary>
    public partial class SessionConfiguration : UserControl
    {

        SiRISViewModel? SiriSViewModel { get; set; }

        public SessionConfiguration()
        {
            InitializeComponent();
            if (DataContext != null)
                SiriSViewModel = (SiRISViewModel)DataContext;
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext != null)
                SiriSViewModel = (SiRISViewModel)DataContext;
            if (e.Key == Key.Enter && SiriSViewModel != null)
                if (SiriSViewModel.SessionManagementViewModel.SessionConfigurationViewModel.SelectAvailableUser(RelationAvailableTextBox.Text))
                    RelationAvailableTextBox.Clear();
        }

        private void TextBoxInserted_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SiriSViewModel != null)
                if (SiriSViewModel.SessionManagementViewModel.SessionConfigurationViewModel.SelectInsertedUser(RelationAvailableTextBox.Text))
                    RelationInsertedTextBox.Clear();
        }
    }
}

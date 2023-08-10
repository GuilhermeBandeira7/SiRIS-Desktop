using SiRISApp.View.Windows;
using SiRISApp.ViewModel;
using SiRISApp.ViewModel.SessionManagement;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiRISApp.View.UserControls.SessionManagement
{
    /// <summary>
    /// Interaction logic for Session.xaml
    /// </summary>
    public partial class Session : UserControl
    {

        SiRISViewModel? SiriSViewModel { get; set; }

        public Session()
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
                if (SiriSViewModel.SessionManagementViewModel.SelectedSession.SelectAvailableUser(RelationAvailableTextBox.Text))
                    RelationAvailableTextBox.Clear();
        }

        private void TextBoxInserted_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SiriSViewModel != null)
                if (SiriSViewModel.SessionManagementViewModel.SelectedSession.SelectInsertedUser(RelationAvailableTextBox.Text))
                    RelationInsertedTextBox.Clear();
        }
    }
}

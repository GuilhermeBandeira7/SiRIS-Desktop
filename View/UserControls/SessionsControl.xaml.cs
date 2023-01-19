using EntityMtwServer.Entities;
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

namespace SiRISApp.View.UserControls
{
    /// <summary>
    /// Interaction logic for SessionsControl.xaml
    /// </summary>
    public partial class SessionsControl : UserControl
    {
        public Session Session
        {
            get { return (Session)GetValue(SessionProperty); }
            set { SetValue(SessionProperty, value); }
        }

        public static readonly DependencyProperty SessionProperty = DependencyProperty.Register("Session", typeof(Session), typeof(SessionsControl), new PropertyMetadata(null, SetValues));

        private static void SetValues(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SessionsControl sessionsControls = (SessionsControl)d;

            if (sessionsControls != null)
            {
                sessionsControls.DataContext = sessionsControls.Session;
            }
        }


        public SessionsControl()
        {
            InitializeComponent();
        }
    }
}

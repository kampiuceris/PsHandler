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

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for WindowGNUGeneralPublicLicense.xaml
    /// </summary>
    public partial class WindowGNUGeneralPublicLicense : Window
    {
        public bool Agrees = false;

        public WindowGNUGeneralPublicLicense(Window owner)
        {
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();
            WebBrowser_Main.NavigateToStream(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("PsHandler.Resources.gnu.html"));
        }

        private void Button_IAgree_Click(object sender, RoutedEventArgs e)
        {
            Agrees = true;
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Agrees = false;
            Close();
        }
    }
}

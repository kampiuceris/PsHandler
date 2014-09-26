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
    /// Interaction logic for WindowManageImportFolders.xaml
    /// </summary>
    public partial class WindowManageImportFolders : Window
    {
        public WindowManageImportFolders(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            TextBox_ImportFolders.Text = Config.ImportFolders.Aggregate("", (current, importFolder) => current + (importFolder + Environment.NewLine));
        }

        private void Button_SaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            Config.ImportFolders = TextBox_ImportFolders.Text.Split(new[] { "\r", "\n", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            Close();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

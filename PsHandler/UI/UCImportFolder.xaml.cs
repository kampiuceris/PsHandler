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

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCImportFolder.xaml
    /// </summary>
    public partial class UCImportFolder : UserControl
    {
        private readonly UCImportFolders _ucImportFolders;
        public string Path;

        public UCImportFolder(UCImportFolders ucImportFolders, string path)
        {
            Path = path;
            _ucImportFolders = ucImportFolders;
            InitializeComponent();
            TextBox_Path.Text = Path;

            TextBox_Path.TextChanged += (sender, args) =>
            {
                Path = TextBox_Path.Text;
                _ucImportFolders.UpdateConfig();
            };
        }

        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            _ucImportFolders.StackPanel_UCFolderPaths.Children.Remove(this);
        }
    }
}

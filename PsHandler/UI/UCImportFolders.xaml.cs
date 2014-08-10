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
    /// Interaction logic for UCImportFolders.xaml
    /// </summary>
    public partial class UCImportFolders : UserControl
    {
        public List<string> FolderPaths = new List<string>();

        public UCImportFolders()
        {
            InitializeComponent();
        }

        public void Seed(List<string> folderPaths)
        {
            FolderPaths = folderPaths;
            foreach (string path in FolderPaths)
            {
                StackPanel_UCFolderPaths.Children.Add(new UCImportFolder(this, path));
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            StackPanel_UCFolderPaths.Children.Add(new UCImportFolder(this, ""));
        }

        public void UpdateConfig()
        {
            Config.ImportFolders.Clear();
            foreach (UCImportFolder ucImportFolder in StackPanel_UCFolderPaths.Children.OfType<UCImportFolder>())
            {
                Config.ImportFolders.Add(ucImportFolder.Path);
            }
        }
    }
}

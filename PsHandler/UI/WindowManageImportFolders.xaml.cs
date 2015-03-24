// PsHandler - poker software helping tool.
// Copyright (C) 2014-2015  kampiuceris

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

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

        private void Button_OK_Click(object sender, RoutedEventArgs e)
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

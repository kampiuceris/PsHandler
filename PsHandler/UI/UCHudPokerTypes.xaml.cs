// PsHandler - poker software helping tool.
// Copyright (C) 2014  kampiuceris

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PsHandler.Custom;
using PsHandler.PokerTypes;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCHudPokerTypes.xaml
    /// </summary>
    public partial class UCHudPokerTypes : UserControl
    {
        public UCHudPokerTypes()
        {
            InitializeComponent();
        }

        public void UpdateListView(PokerType pokerTypeToSelect = null)
        {
            var listView = ListView_PokerTypes;
            listView.Items.Clear();
            foreach (var tableTile in App.PokerTypeManager.GetPokerTypesCopy())
            {
                listView.Items.Add(new ListViewItemPokerType(tableTile));
            }

            if (pokerTypeToSelect != null)
            {
                foreach (var item in listView.Items)
                {
                    if (((ListViewItemPokerType)item).PokerType.Name.Equals(pokerTypeToSelect.Name))
                    {
                        listView.SelectedItem = item;
                        break;
                    }
                }
            }

            GridView_Name.ResetColumnWidths();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            WindowPokerTypeEdit dialog = new WindowPokerTypeEdit(App.WindowMain);
            dialog.ShowDialog();
            if (dialog.Saved)
            {
                App.PokerTypeManager.Add(dialog.PokerType);
                UpdateListView(dialog.PokerType);
            }
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            ListViewItemPokerType selectedItem = ListView_PokerTypes.SelectedItem as ListViewItemPokerType;
            if (selectedItem != null)
            {
                App.PokerTypeManager.Remove(selectedItem.PokerType);
                WindowPokerTypeEdit dialog = new WindowPokerTypeEdit(App.WindowMain, selectedItem.PokerType);
                dialog.ShowDialog();
                App.PokerTypeManager.Add(dialog.PokerType);
                UpdateListView(dialog.PokerType);
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            ListViewItemPokerType selectedItem = ListView_PokerTypes.SelectedItem as ListViewItemPokerType;
            if (selectedItem != null)
            {
                WindowMessageResult windowMessageResult = WindowMessage.ShowDialog(string.Format("Do you want to delete '{0}'?", selectedItem.PokerType.Name), "Delete", WindowMessageButtons.YesNoCancel, WindowMessageImage.Question, App.WindowMain);
                if (windowMessageResult == WindowMessageResult.Yes)
                {
                    App.PokerTypeManager.Remove(selectedItem.PokerType);
                    UpdateListView();
                }
            }
        }

        private void Button_RestoreDefaults_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Do you want to restore default poker type values?");
            sb.AppendLine("This will overwrite only default named poker types.");
            sb.AppendLine("Custom poker types will remain untouched.");
            sb.AppendLine();
            sb.AppendLine("Default poker types:");
            sb.AppendLine();
            List<PokerType> defaultPokerTypes = PokerType.GetDefaultValues().ToList();
            foreach (PokerType pokerType in defaultPokerTypes)
            {
                sb.AppendLine("     " + pokerType.Name);
            }

            WindowMessageResult windowMessageResult = WindowMessage.ShowDialog(
                sb.ToString(),
                "Restore Default Poker Types",
                WindowMessageButtons.YesNoCancel,
                WindowMessageImage.Warning,
                App.WindowMain);

            if (windowMessageResult == WindowMessageResult.Yes)
            {
                foreach (PokerType pokerType in App.PokerTypeManager.GetPokerTypesCopy())
                {
                    if (!defaultPokerTypes.Any(a => a.Name.Equals(pokerType.Name)))
                    {
                        defaultPokerTypes.Add(pokerType);
                    }
                }
                App.PokerTypeManager.RemoveAll();
                App.PokerTypeManager.Add(defaultPokerTypes);
                UpdateListView();
            }
        }

        private void ListView_PokerTypes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button_Edit_Click(null, new RoutedEventArgs());
        }
    }

    public class ListViewItemPokerType : ListViewItem
    {
        public PokerType PokerType;

        public ListViewItemPokerType(PokerType pokerType)
        {
            PokerType = pokerType;

            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };

            Image image = new Image
            {
                Source = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Size16x16/tag_blue.png"), UriKind.Absolute)),
                Width = 16,
                Height = 16,
                Margin = new Thickness(0, 0, 0, 0)
            };

            Label label = new Label
            {
                Content = PokerType.Name,
                Padding = new Thickness(0),
                Margin = new Thickness(5, 0, 0, 0)
            };

            sp.Children.Add(image);
            sp.Children.Add(label);
            Content = sp;
        }
    }
}

﻿// PsHandler - poker software helping tool.
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

using System.Globalization;
using PsHandler.Custom;
using PsHandler.TableTiler;
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
    /// Interaction logic for UCTableTiler.xaml
    /// </summary>
    public partial class UCTableTiler : UserControl
    {
        public UCTableTiler()
        {
            InitializeComponent();

            // Seed

            TextBox_AutoTileCheckingTimeMs.Text = Config.AutoTileCheckingTimeMs.ToString(CultureInfo.InvariantCulture);

            // Hook values

            CheckBox_EnableTableTimer.Checked += (sender, args) => { Config.EnableTableTiler = true; };
            CheckBox_EnableTableTimer.Unchecked += (sender, args) => { Config.EnableTableTiler = false; };
            TextBox_AutoTileCheckingTimeMs.TextChanged += (sender, args) =>
            {
                try
                {
                    Config.AutoTileCheckingTimeMs = int.Parse(TextBox_AutoTileCheckingTimeMs.Text);
                    if (Config.AutoTileCheckingTimeMs > 10000) Config.AutoTileCheckingTimeMs = 10000;
                    if (Config.AutoTileCheckingTimeMs < 0) Config.AutoTileCheckingTimeMs = 0;
                }
                catch
                {
                }
            };

            // ToolTips

            Label_AutoTileCheckingTime.ToolTip = "When new table opens its title changes few times in first seconds." + Environment.NewLine +
                "Therefore Table Tiler constantly checks for given time until it finds matching auto " + Environment.NewLine +
                "tile config or removes the new table from queue. Recomended: 3000 (3 second).";
            ToolTipService.SetShowDuration(Label_AutoTileCheckingTime, 60000);

            // start table tiler if needed
            CheckBox_EnableTableTimer.IsChecked = Config.EnableTableTiler;
        }

        public void UpdateListView(TableTile tableTileToSelect = null)
        {
            var listView = ListView_TableTiles;
            listView.Items.Clear();
            foreach (var tableTile in App.TableTileManager.GetTableTilesCopy())
            {
                listView.Items.Add(new ListViewItemTableTile(tableTile));
            }

            if (tableTileToSelect != null)
            {
                foreach (var item in listView.Items)
                {
                    if (((ListViewItemTableTile)item).TableTile.Name.Equals(tableTileToSelect.Name))
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
            WindowTableTileEdit dialog = new WindowTableTileEdit(App.WindowMain);
            dialog.ShowDialog();
            if (dialog.Saved)
            {
                App.TableTileManager.Add(dialog.TableTile);
                UpdateListView(dialog.TableTile);
            }
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            ListViewItemTableTile selectedItem = ListView_TableTiles.SelectedItem as ListViewItemTableTile;
            if (selectedItem != null)
            {
                App.TableTileManager.Remove(selectedItem.TableTile);
                WindowTableTileEdit dialog = new WindowTableTileEdit(App.WindowMain, selectedItem.TableTile);
                dialog.ShowDialog();
                App.TableTileManager.Add(dialog.TableTile);
                UpdateListView(dialog.TableTile);
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            ListViewItemTableTile selectedItem = ListView_TableTiles.SelectedItem as ListViewItemTableTile;
            if (selectedItem != null)
            {
                WindowMessageResult result = WindowMessage.ShowDialog(string.Format("Do you want to delete '{0}'?", selectedItem.TableTile.Name), "Delete", WindowMessageButtons.YesNoCancel, WindowMessageImage.Question, App.WindowMain);
                if (result == WindowMessageResult.Yes)
                {
                    App.TableTileManager.Remove(selectedItem.TableTile);
                    UpdateListView();
                }
            }
        }

        private void ListView_TableTilerConfigs_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button_Edit_Click(null, new RoutedEventArgs());
        }

        private void Button_RestoreDefaults_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Do you want to restore default table tile configs?");
            sb.AppendLine("This will overwrite only default named table tile configs.");
            sb.AppendLine("Custom table tile cofigs will remain untouched.");
            sb.AppendLine();
            sb.AppendLine("Default table tile configs:");
            sb.AppendLine();
            List<TableTile> defaultTableTiles = TableTile.GetDefaultValues().ToList();
            foreach (TableTile tableTile in defaultTableTiles)
            {
                sb.AppendLine("     " + tableTile.Name);
            }

            WindowMessageResult windowMessageResult = WindowMessage.ShowDialog(
                sb.ToString(),
                "Restore Default Table Tile Configs",
                WindowMessageButtons.YesNoCancel,
                WindowMessageImage.Warning,
                App.WindowMain);

            if (windowMessageResult == WindowMessageResult.Yes)
            {
                foreach (TableTile tableTile in App.TableTileManager.GetTableTilesCopy())
                {
                    if (!defaultTableTiles.Any(a => a.Name.Equals(tableTile.Name)))
                    {
                        defaultTableTiles.Add(tableTile);
                    }
                }
                App.TableTileManager.RemoveAll();
                App.TableTileManager.Add(defaultTableTiles);
                UpdateListView();
            }
        }

        private void Button_CloneSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ListView_TableTiles.SelectedItem as ListViewItemTableTile;
            if (selectedItem != null)
            {
                var fromXElement = TableTile.FromXElement(selectedItem.TableTile.ToXElement());
                if (fromXElement != null)
                {
                    fromXElement.Name = fromXElement.Name + string.Format(" {0}", DateTime.Now.Ticks);
                    App.TableTileManager.Add(fromXElement);
                    UpdateListView();
                }
            }
        }
    }

    public class ListViewItemTableTile : ListViewItem
    {
        public TableTile TableTile;

        public ListViewItemTableTile(TableTile tableTile)
        {
            TableTile = tableTile;

            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };

            CheckBox checkBox = new CheckBox
            {
                Content = "",
                IsChecked = TableTile.IsEnabled,
                Margin = new Thickness(0, 0, 0, 0)
            };
            checkBox.Checked += (sender, args) => TableTile.IsEnabled = true;
            checkBox.Unchecked += (sender, args) => TableTile.IsEnabled = false;

            Image image = new Image
            {
                Source = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Size16x16/application_cascade.png"), UriKind.Absolute)),
                Width = 16,
                Height = 16,
                Margin = new Thickness(5, 0, 0, 0)
            };

            Label label = new Label
            {
                Content = TableTile.Name,
                Padding = new Thickness(0),
                Margin = new Thickness(5, 0, 0, 0)
            };

            sp.Children.Add(checkBox);
            sp.Children.Add(image);
            sp.Children.Add(label);
            Content = sp;
        }
    }
}

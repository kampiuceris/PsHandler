﻿using PsHandler.Custom;
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

            // Hook values

            CheckBox_EnableTableTimer.Checked += (sender, args) => { Config.EnableTableTiler = true; };
            CheckBox_EnableTableTimer.Unchecked += (sender, args) => { Config.EnableTableTiler = false; };

            // start table tiler if needed
            CheckBox_EnableTableTimer.IsChecked = Config.EnableTableTiler;
        }

        public void UpdateListView(TableTile tableTileToSelect = null)
        {
            var listView = ListView_TableTiles;
            listView.Items.Clear();
            foreach (var tableTile in TableTileManager.GetTableTilesCopy())
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
                TableTileManager.Add(dialog.TableTile);
                UpdateListView(dialog.TableTile);
            }
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            ListViewItemTableTile selectedItem = ListView_TableTiles.SelectedItem as ListViewItemTableTile;
            if (selectedItem != null)
            {
                TableTileManager.Remove(selectedItem.TableTile);
                WindowTableTileEdit dialog = new WindowTableTileEdit(App.WindowMain, selectedItem.TableTile);
                dialog.ShowDialog();
                TableTileManager.Add(dialog.TableTile);
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
                    TableTileManager.Remove(selectedItem.TableTile);
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
                foreach (TableTile tableTile in TableTileManager.GetTableTilesCopy())
                {
                    if (!defaultTableTiles.Any(a => a.Name.Equals(tableTile.Name)))
                    {
                        defaultTableTiles.Add(tableTile);
                    }
                }
                TableTileManager.RemoveAll();
                TableTileManager.Add(defaultTableTiles);
                UpdateListView();
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
                Source = Methods.GetEmbeddedResourceBitmap(string.Format("PsHandler.Images.EmbeddedResources.Size16x16.application_cascade.png")).ToBitmapSource(),
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

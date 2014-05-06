using PsHandler.TableTiler;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            Loaded += (sender, args) => UpdateListView();
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
                MessageBoxResult messageBoxResult = MessageBox.Show(string.Format("Do you want to delete '{0}'?", selectedItem.TableTile.Name), "Delete", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
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

        public static void UpdateListView(TableTile tableTileToSelect = null)
        {
            var listView = App.WindowMain.UCTableTiler.ListView_TableTiles;
            listView.Items.Clear();
            lock (TableTileManager.Lock)
            {
                foreach (var tableTile in TableTileManager.TableTiles)
                {
                    listView.Items.Add(new ListViewItemTableTile(tableTile));
                }
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

            App.WindowMain.UCTableTiler.GridView_TableTiles.ResetColumnWidths();
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
                Source = Methods.GetEmbeddedResourceBitmap(string.Format("PsHandler.Images.application_cascade.png")).ToBitmapSource(),
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsHandler.PokerTypes
{
    /// <summary>
    /// Interaction logic for UCPokerTypes.xaml
    /// </summary>
    public partial class UCPokerTypes : UserControl
    {
        public UCPokerTypes()
        {
            InitializeComponent();
        }

        public static void UpdateListView(PokerType pokerTypeToSelect = null)
        {
            var listView = App.WindowMain.UCHud.UCPokerTypes.ListView_PokerTypes;
            listView.Items.Clear();
            lock (PokerTypeManager.Lock)
            {
                foreach (var tableTile in PokerTypeManager.PokerTypes)
                {
                    listView.Items.Add(new ListViewItemPokerType(tableTile));
                }
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

            App.WindowMain.UCHud.UCPokerTypes.GridView_Name.ResetColumnWidths();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            WindowPokerTypeEdit dialog = new WindowPokerTypeEdit(App.WindowMain);
            dialog.ShowDialog();
            if (dialog.Saved)
            {
                PokerTypeManager.Add(dialog.PokerType);
                UpdateListView(dialog.PokerType);
            }
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            ListViewItemPokerType selectedItem = ListView_PokerTypes.SelectedItem as ListViewItemPokerType;
            if (selectedItem != null)
            {
                PokerTypeManager.Remove(selectedItem.PokerType);
                WindowPokerTypeEdit dialog = new WindowPokerTypeEdit(App.WindowMain, selectedItem.PokerType);
                dialog.ShowDialog();
                PokerTypeManager.Add(dialog.PokerType);
                UpdateListView(dialog.PokerType);
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            ListViewItemPokerType selectedItem = ListView_PokerTypes.SelectedItem as ListViewItemPokerType;
            if (selectedItem != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(string.Format("Do you want to delete '{0}'?", selectedItem.PokerType.Name), "Delete", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    PokerTypeManager.Remove(selectedItem.PokerType);
                    UpdateListView();
                }
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
                Source = Methods.GetEmbeddedResourceBitmap(string.Format("PsHandler.Images.EmbeddedResources.tag_blue.png")).ToBitmapSource(),
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

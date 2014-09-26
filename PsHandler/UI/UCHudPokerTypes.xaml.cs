﻿using System;
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
            foreach (var tableTile in PokerTypeManager.GetPokerTypesCopy())
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
                WindowMessageResult windowMessageResult = WindowMessage.ShowDialog(string.Format("Do you want to delete '{0}'?", selectedItem.PokerType.Name), "Delete", WindowMessageButtons.YesNoCancel, WindowMessageImage.Question, App.WindowMain);
                if (windowMessageResult == WindowMessageResult.Yes)
                {
                    PokerTypeManager.Remove(selectedItem.PokerType);
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
                foreach (PokerType pokerType in PokerTypeManager.GetPokerTypesCopy())
                {
                    if (!defaultPokerTypes.Any(a => a.Name.Equals(pokerType.Name)))
                    {
                        defaultPokerTypes.Add(pokerType);
                    }
                }
                PokerTypeManager.RemoveAll();
                PokerTypeManager.Add(defaultPokerTypes);
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
                Source = Methods.GetEmbeddedResourceBitmap(string.Format("PsHandler.Images.EmbeddedResources.Size16x16.tag_blue.png")).ToBitmapSource(),
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
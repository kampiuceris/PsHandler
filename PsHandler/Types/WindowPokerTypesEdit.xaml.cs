using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace PsHandler.Types
{
    /// <summary>
    /// Interaction logic for WindowPokerTypesEdit.xaml
    /// </summary>
    public partial class WindowPokerTypesEdit : Window
    {
        public List<PokerType> PokerTypes = new List<PokerType>();

        public WindowPokerTypesEdit(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            try
            {
                using (RegistryKey keyPokerTypes = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler\PokerTypes", true))
                {
                    foreach (string valueName in keyPokerTypes.GetValueNames())
                    {
                        PokerType pokerType = PokerType.FromXml(keyPokerTypes.GetValue(valueName) as string);
                        if (pokerType != null)
                        {
                            PokerTypes.Add(pokerType);
                        }
                    }

                    UpdateListViewView();
                }
            }
            catch (Exception)
            {
            }
        }

        private void UpdateListViewView()
        {
            PokerType selectedItem = ListView_PokerTypes.SelectedItem as PokerType;
            ListView_PokerTypes.Items.Clear();
            PokerTypes.Sort((o0, o1) => string.CompareOrdinal(o0.Name, o1.Name));
            foreach (PokerType pokerType in PokerTypes) ListView_PokerTypes.Items.Add(pokerType);
            if (selectedItem != null)
            {
                ListView_PokerTypes.SelectedItem = selectedItem;
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            WindowPokerTypeEdit windowPokerTypeEdit = new WindowPokerTypeEdit();
            windowPokerTypeEdit.ShowDialog();
            if (windowPokerTypeEdit.Saved)
            {
                PokerTypes.Add(windowPokerTypeEdit.PokerType);
                UpdateListViewView();
            }
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            PokerType selectedItem = ListView_PokerTypes.SelectedItem as PokerType;
            if (selectedItem != null)
            {
                WindowPokerTypeEdit windowPokerTypeEdit = new WindowPokerTypeEdit(selectedItem);
                windowPokerTypeEdit.ShowDialog();
                UpdateListViewView();
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            PokerType selectedItem = ListView_PokerTypes.SelectedItem as PokerType;
            if (selectedItem != null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(string.Format("Do you want to delete '{0}'?", selectedItem.Name), "Delete", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    PokerTypes.Remove(selectedItem);
                    UpdateListViewView();
                }
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_SaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            // check if all names are different
            if (PokerTypes.Any(pt => PokerTypes.Count(pt1 => pt1.Name.Equals(pt.Name)) != 1))
            {
                MessageBox.Show("Cannot save. Duplicate names.", "Error saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // save to registry
            try
            {
                using (RegistryKey keyPokerTypes = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler\PokerTypes", true))
                {
                    foreach (string valueName in keyPokerTypes.GetValueNames())
                    {
                        keyPokerTypes.DeleteValue(valueName);
                    }

                    foreach (PokerType pokerType in PokerTypes)
                    {
                        keyPokerTypes.SetValue(pokerType.Name, pokerType.ToXml());
                    }
                }
            }
            catch (Exception)
            {
            }

            Close();
        }

        private void ListView_PokerTypes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button_Edit_Click(null, new RoutedEventArgs());
        }
    }
}

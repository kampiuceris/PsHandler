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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCSettings.xaml
    /// </summary>
    public partial class UCSettings : UserControl
    {
        public UCSettings()
        {
            InitializeComponent();

            // Init values

            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Unknown());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Azure());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Black());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Classic());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.HyperSimple());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Mercury());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Nova());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Slick());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Stars());
            if (Config.PokerStarsThemeTable != null)
            {
                foreach (var item in ComboBox_PokerStarsThemeTable.Items)
                {
                    if (Config.PokerStarsThemeTable.GetType() == item.GetType())
                    {
                        ComboBox_PokerStarsThemeTable.SelectedItem = item;
                        break;
                    }
                }
            }

            CheckBox_MinimizeToSystemTray.IsChecked = Config.MinimizeToSystemTray;

            CheckBox_StartMinimized.IsChecked = Config.StartMinimized;

            TextBoxHotkey_Exit.KeyCombination = Config.HotkeyExit;

            CheckBox_SaveGuiLocation.IsChecked = Config.SaveGuiLocation;

            CheckBox_SaveGuiSize.IsChecked = Config.SaveGuiSize;

            // Hook values

            ComboBox_PokerStarsThemeTable.SelectionChanged += (sender, args) =>
            {
                var value = ComboBox_PokerStarsThemeTable.SelectedItem as PokerStarsThemeTable;
                Config.PokerStarsThemeTable = value ?? new PokerStarsThemesTable.Unknown();
            };

            CheckBox_MinimizeToSystemTray.Checked += (sender, args) => { Config.MinimizeToSystemTray = true; };
            CheckBox_MinimizeToSystemTray.Unchecked += (sender, args) => { Config.MinimizeToSystemTray = false; };

            CheckBox_StartMinimized.Checked += (sender, args) => { Config.StartMinimized = true; };
            CheckBox_StartMinimized.Unchecked += (sender, args) => { Config.StartMinimized = false; };

            TextBoxHotkey_Exit.TextChanged += (sender, args) => { Config.HotkeyExit = TextBoxHotkey_Exit.KeyCombination; };

            CheckBox_SaveGuiLocation.Checked += (sender, args) => { Config.SaveGuiLocation = true; };
            CheckBox_SaveGuiLocation.Unchecked += (sender, args) => { Config.SaveGuiLocation = false; };

            CheckBox_SaveGuiSize.Checked += (sender, args) => { Config.SaveGuiSize = true; };
            CheckBox_SaveGuiSize.Unchecked += (sender, args) => { Config.SaveGuiSize = false; };

            // ToolTips

            //Image_PokerStarsThemeTable.ToolTip = new UCToolTipPokerStarsThemeTable();
            //Image_ExitHotkey.ToolTip = new UCToolTipExitHotkey();
        }

        private void Button_ManageImportFolders_Click(object sender, RoutedEventArgs e)
        {
            WindowManageImportFolders dialog = new WindowManageImportFolders(App.WindowMain);
            dialog.ShowDialog();
        }

        private void Button_ManagePreferredSeats_Click(object sender, RoutedEventArgs e)
        {
            new WindowManagePreferredSeats(App.WindowMain).ShowDialog();
        }
    }
}

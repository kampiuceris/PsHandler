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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using PsHandler.Hud;
using PsHandler.PokerMath;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCHudBigBlind.xaml
    /// </summary>
    public partial class UCHudBigBlind : UserControl
    {
        public UCHudBigBlind()
        {
            InitializeComponent();

            // Init values

            CheckBox_EnableHudBigBlind.IsChecked = Config.HudBigBlindEnable;
            RadioButton_MByPlayerCount.IsChecked = Config.HudBigBlindMByPlayerCount;
            RadioButton_MByTableSize.IsChecked = Config.HudBigBlindMByTableSize;
            TextBox_Decimals.Text = Config.HudBigBlindDecimals.ToString(CultureInfo.InvariantCulture);
            TextBox_BigBlindHHNotFound.Text = Config.HudBigBlindHHNotFound;
            TextBox_BigBlindPrefix.Text = Config.HudBigBlindPrefix;
            TextBox_BigBlindPostfix.Text = Config.HudBigBlindPostfix;
            CheckBox_LockHudBigBlindLocation.IsChecked = Config.HudBigBlindLocationLocked;
            CheckBox_ShowForOpponents.IsChecked = Config.HudBigBlindShowForOpponents;
            CheckBox_ShowForHero.IsChecked = Config.HudBigBlindShowForHero;

            // Hook values

            CheckBox_EnableHudBigBlind.Checked += (sender, args) => { Config.HudBigBlindEnable = true; };
            CheckBox_EnableHudBigBlind.Unchecked += (sender, args) => { Config.HudBigBlindEnable = false; };

            CheckBox_LockHudBigBlindLocation.Checked += (sender, args) => { Config.HudBigBlindLocationLocked = true; };
            CheckBox_LockHudBigBlindLocation.Unchecked += (sender, args) => { Config.HudBigBlindLocationLocked = false; };

            RadioButton_ShowBB.Checked += (sender, args) =>
            {
                Config.HudBigBlindShowBB = true;
                Config.HudBigBlindShowAdjustedBB = false;
                Config.HudBigBlindShowTournamentM = false;
                RadioButton_MByPlayerCount.IsEnabled = false;
                RadioButton_MByTableSize.IsEnabled = false;
            };
            RadioButton_ShowAdjustedBB.Checked += (sender, args) =>
            {
                Config.HudBigBlindShowBB = false;
                Config.HudBigBlindShowAdjustedBB = true;
                Config.HudBigBlindShowTournamentM = false;
                RadioButton_MByPlayerCount.IsEnabled = true;
                RadioButton_MByTableSize.IsEnabled = true;
            };
            RadioButton_ShowTournamentM.Checked += (sender, args) =>
            {
                Config.HudBigBlindShowBB = false;
                Config.HudBigBlindShowAdjustedBB = false;
                Config.HudBigBlindShowTournamentM = true;
                RadioButton_MByPlayerCount.IsEnabled = true;
                RadioButton_MByTableSize.IsEnabled = true;
            };

            RadioButton_MByPlayerCount.Checked += (sender, args) => { Config.HudBigBlindMByPlayerCount = true; };
            RadioButton_MByPlayerCount.Unchecked += (sender, args) => { Config.HudBigBlindMByPlayerCount = false; };
            RadioButton_MByTableSize.Checked += (sender, args) => { Config.HudBigBlindMByTableSize = true; };
            RadioButton_MByTableSize.Unchecked += (sender, args) => { Config.HudBigBlindMByTableSize = false; };

            CheckBox_ShowForOpponents.Checked += (sender, args) => { Config.HudBigBlindShowForOpponents = true; };
            CheckBox_ShowForOpponents.Unchecked += (sender, args) => { Config.HudBigBlindShowForOpponents = false; };
            CheckBox_ShowForHero.Checked += (sender, args) => { Config.HudBigBlindShowForHero = true; };
            CheckBox_ShowForHero.Unchecked += (sender, args) => { Config.HudBigBlindShowForHero = false; };

            TextBox_Decimals.TextChanged += (sender, args) =>
            {
                try
                {
                    Config.HudBigBlindDecimals = int.Parse(TextBox_Decimals.Text);
                    if (Config.HudBigBlindDecimals > 4) Config.HudBigBlindDecimals = 4;
                    if (Config.HudBigBlindDecimals < 0) Config.HudBigBlindDecimals = 0;
                }
                catch
                {
                }
            };

            TextBox_BigBlindHHNotFound.TextChanged += (sender, args) => { Config.HudBigBlindHHNotFound = TextBox_BigBlindHHNotFound.Text; };
            TextBox_BigBlindPrefix.TextChanged += (sender, args) => { Config.HudBigBlindPrefix = TextBox_BigBlindPrefix.Text; };
            TextBox_BigBlindPostfix.TextChanged += (sender, args) => { Config.HudBigBlindPostfix = TextBox_BigBlindPostfix.Text; };

            // Hook needed init values

            RadioButton_ShowBB.IsChecked = Config.HudBigBlindShowBB;
            RadioButton_ShowAdjustedBB.IsChecked = Config.HudBigBlindShowAdjustedBB;
            RadioButton_ShowTournamentM.IsChecked = Config.HudBigBlindShowTournamentM;

            // ToolTips
        }

        private void Button_CustomizeOpponents_Click(object sender, RoutedEventArgs e)
        {
            new WindowHudDesign(App.WindowMain, HudCustomizeParams.HudCustomizeType.HudBigBlindOpponents).ShowDialog();
        }

        private void Button_CustomizeHero_Click(object sender, RoutedEventArgs e)
        {
            new WindowHudDesign(App.WindowMain, HudCustomizeParams.HudCustomizeType.HudBigBlindHero).ShowDialog();
        }

        private void Button_ColorsByValueOpponents_Click(object sender, RoutedEventArgs e)
        {
            new WindowCustomizeColorsByValue(App.WindowMain, Config.HudBigBlindOpponentsForeground, HudColorsByValueParams.HudColorsByValueType.HudColorsByValueOpponents).ShowDialog();
        }

        private void Button_ColorsByValueHero_Click(object sender, RoutedEventArgs e)
        {
            new WindowCustomizeColorsByValue(App.WindowMain, Config.HudBigBlindHeroForeground, HudColorsByValueParams.HudColorsByValueType.HudColorsByValueHero).ShowDialog();
        }

        private void Button_RestoreDefaultLocations_Click(object sender, RoutedEventArgs e)
        {
            WindowMessageResult windowMessageResult = WindowMessage.ShowDialog(
                "Do you want to restore default locations?",
                "Restore Default Locations",
                WindowMessageButtons.YesNoCancel,
                WindowMessageImage.Warning,
                App.WindowMain);

            if (windowMessageResult == WindowMessageResult.Yes)
            {
                for (int tableSize = 0; tableSize < 11; tableSize++)
                {
                    for (int position = 0; position < 10; position++)
                    {
                        Config.HudBigBlindLocationsX[tableSize][position] = Config.DefaultHudBigBlindLocationsX[tableSize][position];
                        Config.HudBigBlindLocationsY[tableSize][position] = Config.DefaultHudBigBlindLocationsY[tableSize][position];
                    }
                }
            }
        }
    }
}

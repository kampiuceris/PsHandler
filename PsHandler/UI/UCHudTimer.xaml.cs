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
using PsHandler.Hud;
using PsHandler.Import;
using PsHandler.PokerMath;
using PsHandler.UI.ToolTips;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCHudTimer.xaml
    /// </summary>
    public partial class UCHudTimer : UserControl
    {
        public UCHudTimer()
        {
            InitializeComponent();

            // Init values

            CheckBox_EnableHudTimer.IsChecked = Config.HudTimerEnable;
            TextBox_TimerDiff.Text = Config.HudTimerDiff.ToString(CultureInfo.InvariantCulture);
            TextBox_TimerHHNotFound.Text = Config.HudTimerHHNotFound;
            TextBox_TimerPokerTypeNotFound.Text = Config.HudTimerPokerTypeNotFound;
            TextBox_TimerMultiplePokerTypes.Text = Config.HudTimerMultiplePokerTypes;
            CheckBox_LockHudTimerLocation.IsChecked = Config.HudTimerLocationLocked;

            // Hook values

            CheckBox_EnableHudTimer.Checked += (sender, args) => { Config.HudTimerEnable = true; };
            CheckBox_EnableHudTimer.Unchecked += (sender, args) => { Config.HudTimerEnable = false; };

            RadioButton_ShowTimer.Checked += (sender, args) =>
            {
                Config.HudTimerShowTimer = true;
                Config.HudTimerShowHandCount = false;
                Label_TimerDifference.IsEnabled = true;
                TextBox_TimerDiff.IsEnabled = true;
            };
            RadioButton_ShowHandCount.Checked += (sender, args) =>
            {
                Config.HudTimerShowTimer = false;
                Config.HudTimerShowHandCount = true;
                Label_TimerDifference.IsEnabled = false;
                TextBox_TimerDiff.IsEnabled = false;
            };

            TextBox_TimerDiff.TextChanged += (sender, args) => int.TryParse(TextBox_TimerDiff.Text, out Config.HudTimerDiff);
            TextBox_TimerHHNotFound.TextChanged += (sender, args) => Config.HudTimerHHNotFound = TextBox_TimerHHNotFound.Text;
            TextBox_TimerPokerTypeNotFound.TextChanged += (sender, args) => Config.HudTimerPokerTypeNotFound = TextBox_TimerPokerTypeNotFound.Text;
            TextBox_TimerMultiplePokerTypes.TextChanged += (sender, args) => Config.HudTimerMultiplePokerTypes = TextBox_TimerMultiplePokerTypes.Text;

            CheckBox_LockHudTimerLocation.Checked += (sender, args) => { Config.HudTimerLocationLocked = true; };
            CheckBox_LockHudTimerLocation.Unchecked += (sender, args) => { Config.HudTimerLocationLocked = false; };

            // Hook needed init values

            RadioButton_ShowTimer.IsChecked = Config.HudTimerShowTimer;
            RadioButton_ShowHandCount.IsChecked = Config.HudTimerShowHandCount;

            // ToolTips

            Label_TimerDifference.ToolTip = new UCToolTipHudTimerTimeDifference();
            ToolTipService.SetShowDuration(Label_TimerDifference, 60000);

            RadioButton_ShowHandCount.ToolTip = "Shows hand count from the level start. This is useful for Hyper Turbo Sat players where levels increase by hand count instead of time.";
            ToolTipService.SetShowDuration(RadioButton_ShowHandCount, 60000);
        }

        private void Button_Customize_Click(object sender, RoutedEventArgs e)
        {
            new WindowHudDesign(App.WindowMain, HudCustomizeParams.HudCustomizeType.HudTimer).ShowDialog();
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
                    Config.HudTimerLocationsX[tableSize] = Config.DefaultHudTimerLocationsX[tableSize];
                    Config.HudTimerLocationsY[tableSize] = Config.DefaultHudTimerLocationsY[tableSize];
                }
            }
        }
    }
}

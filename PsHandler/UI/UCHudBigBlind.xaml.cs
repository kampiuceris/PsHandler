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

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCHudBigBlind.xaml
    /// </summary>
    public partial class UCHudBigBlind : UserControl
    {
        public TextBox[] TextBoxesLocationX;
        public TextBox[] TextBoxesLocationY;

        public UCHudBigBlind()
        {
            InitializeComponent();

            TextBoxesLocationX = new List<TextBox>
            {
                TextBox_HudBigBlindLocationX_Default,
                TextBox_HudBigBlindLocationX_Max1,
                TextBox_HudBigBlindLocationX_Max2,
                TextBox_HudBigBlindLocationX_Max3,
                TextBox_HudBigBlindLocationX_Max4,
                TextBox_HudBigBlindLocationX_Max5,
                TextBox_HudBigBlindLocationX_Max6,
                TextBox_HudBigBlindLocationX_Max7,
                TextBox_HudBigBlindLocationX_Max8,
                TextBox_HudBigBlindLocationX_Max9,
                TextBox_HudBigBlindLocationX_Max10
            }.ToArray();
            TextBoxesLocationY = new List<TextBox>
            {
                TextBox_HudBigBlindLocationY_Default,
                TextBox_HudBigBlindLocationY_Max1,
                TextBox_HudBigBlindLocationY_Max2,
                TextBox_HudBigBlindLocationY_Max3,
                TextBox_HudBigBlindLocationY_Max4,
                TextBox_HudBigBlindLocationY_Max5,
                TextBox_HudBigBlindLocationY_Max6,
                TextBox_HudBigBlindLocationY_Max7,
                TextBox_HudBigBlindLocationY_Max8,
                TextBox_HudBigBlindLocationY_Max9,
                TextBox_HudBigBlindLocationY_Max10
            }.ToArray();

            // Init values

            CheckBox_EnableHudBigBlind.IsChecked = TableManager.EnableHudBigBlind;
            RadioButton_MByPlayerCount.IsChecked = Config.BigBlindMByPlayerCount;
            RadioButton_MByTableSize.IsChecked = Config.BigBlindMByTableSize;
            TextBox_Decimals.Text = Config.BigBlindDecimals.ToString(CultureInfo.InvariantCulture);
            TextBox_BigBlindHHNotFound.Text = Config.BigBlindHHNotFound;
            TextBox_BigBlindPrefix.Text = Config.BigBlindPrefix;
            TextBox_BigBlindPostfix.Text = Config.BigBlindPostfix;
            for (int i = 0; i < TextBoxesLocationX.Length; i++)
            {
                TextBoxesLocationX[i].Text = TableManager.GetHudBigBlindLocationX((TableSize)i, TextBoxesLocationX[i]).ToString(CultureInfo.InvariantCulture);
                TextBoxesLocationY[i].Text = TableManager.GetHudBigBlindLocationY((TableSize)i, TextBoxesLocationY[i]).ToString(CultureInfo.InvariantCulture);
            }

            // Hook values

            CheckBox_EnableHudBigBlind.Checked += (sender, args) => { TableManager.EnableHudBigBlind = true; };
            CheckBox_EnableHudBigBlind.Unchecked += (sender, args) => { TableManager.EnableHudBigBlind = false; };
            CheckBox_ShowTournamentM.Checked += (sender, args) =>
            {
                Config.BigBlindShowTournamentM = true;
                RadioButton_MByPlayerCount.IsEnabled = true;
                RadioButton_MByTableSize.IsEnabled = true;
            };
            CheckBox_ShowTournamentM.Unchecked += (sender, args) =>
            {
                Config.BigBlindShowTournamentM = false;
                RadioButton_MByPlayerCount.IsEnabled = false;
                RadioButton_MByTableSize.IsEnabled = false;
            };
            RadioButton_MByPlayerCount.Checked += (sender, args) => { Config.BigBlindMByPlayerCount = true; };
            RadioButton_MByPlayerCount.Unchecked += (sender, args) => { Config.BigBlindMByPlayerCount = false; };
            RadioButton_MByTableSize.Checked += (sender, args) => { Config.BigBlindMByTableSize = true; };
            RadioButton_MByTableSize.Unchecked += (sender, args) => { Config.BigBlindMByTableSize = false; };

            TextBox_Decimals.TextChanged += (sender, args) =>
            {
                try
                {
                    Config.BigBlindDecimals = int.Parse(TextBox_Decimals.Text);
                    if (Config.BigBlindDecimals > 4) Config.BigBlindDecimals = 4;
                    if (Config.BigBlindDecimals < 0) Config.BigBlindDecimals = 0;
                }
                catch
                {
                }
            };

            TextBox_BigBlindHHNotFound.TextChanged += (sender, args) => { Config.BigBlindHHNotFound = TextBox_BigBlindHHNotFound.Text; };
            TextBox_BigBlindPrefix.TextChanged += (sender, args) => { Config.BigBlindPrefix = TextBox_BigBlindPrefix.Text; };
            TextBox_BigBlindPostfix.TextChanged += (sender, args) => { Config.BigBlindPostfix = TextBox_BigBlindPostfix.Text; };

            CheckBox_LockHudBigBlindLocation.Checked += (sender, args) =>
            {
                TableManager.HudBigBlindLocationLocked = true;
                for (int i = 0; i < TextBoxesLocationX.Length; i++)
                {
                    TextBoxesLocationX[i].IsEnabled = false;
                    TextBoxesLocationY[i].IsEnabled = false;
                }
            };
            CheckBox_LockHudBigBlindLocation.Unchecked += (sender, args) =>
            {
                TableManager.HudBigBlindLocationLocked = false;
                for (int i = 0; i < TextBoxesLocationX.Length; i++)
                {
                    TextBoxesLocationX[i].IsEnabled = true;
                    TextBoxesLocationY[i].IsEnabled = true;
                }
            };

            for (int i = 0; i < TextBoxesLocationX.Length; i++)
            {
                int i1 = i;
                TextBoxesLocationX[i].TextChanged += (sender, args) =>
                {
                    float f;
                    if (float.TryParse(TextBoxesLocationX[i1].Text, out f))
                    {
                        TableManager.SetHudBigBlindLocationX((TableSize)i1, f, TextBoxesLocationX[i1]);
                    }
                };
                TextBoxesLocationY[i].TextChanged += (sender, args) =>
                {
                    float f;
                    if (float.TryParse(TextBoxesLocationY[i1].Text, out f))
                    {
                        TableManager.SetHudBigBlindLocationY((TableSize)i1, f, TextBoxesLocationY[i1]);
                    }
                };
            }

            // hook needed init values

            CheckBox_LockHudBigBlindLocation.IsChecked = TableManager.HudBigBlindLocationLocked;
            CheckBox_ShowTournamentM.IsChecked = Config.BigBlindShowTournamentM;

            // ToolTips
        }

        private void Button_Customize_Click(object sender, RoutedEventArgs e)
        {
            string decimalsFormal = "";
            for (int i = 0; i < Config.BigBlindDecimals; i++) decimalsFormal += "0";
            if (decimalsFormal.Length > 0) decimalsFormal = "." + decimalsFormal;
            string x = string.Format("{0:0" + decimalsFormal + "}", 12.3456); // 12.3456 test data

            var dialog = new WindowHudDesign(
                App.WindowMain,
                Config.HudBigBlindBackground,
                Config.HudBigBlindForeground,
                Config.HudBigBlindFontFamily,
                Config.HudBigBlindFontWeight,
                Config.HudBigBlindFontStyle,
                Config.HudBigBlindFontSize,
                Config.HudBigBlindMargin,
                x);
            dialog.ShowDialog();
            if (dialog.Saved)
            {
                Config.HudBigBlindBackground = dialog.HudBackground;
                Config.HudBigBlindForeground = dialog.HudForeground;
                Config.HudBigBlindFontFamily = dialog.HudFontFamily;
                Config.HudBigBlindFontWeight = dialog.HudFontWeight;
                Config.HudBigBlindFontStyle = dialog.HudFontStyle;
                Config.HudBigBlindFontSize = dialog.HudFontSize;
                Config.HudBigBlindMargin = dialog.HudMargin;
            }
        }

        private void Button_CustomizeColorsByValue_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WindowCustomizeColorsByValue(App.WindowMain, Config.HudBigBlindForeground, Config.HudBigBlindColorsByValue);
            dialog.ShowDialog();
            if (dialog.Saved)
            {
                Config.HudBigBlindColorsByValue = dialog.ColorsByValue;
            }
        }
    }
}

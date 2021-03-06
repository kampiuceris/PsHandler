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
using PsHandler.UI.ToolTips;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCController.xaml
    /// </summary>
    public partial class UCController : UserControl
    {
        public UCController()
        {
            InitializeComponent();

            // Hook

            CheckBox_AutoclickImBack.Checked += (sender, args) => { Config.AutoclickImBack = true; };
            CheckBox_AutoclickImBack.Unchecked += (sender, args) => { Config.AutoclickImBack = false; };
            CheckBox_AutoclickTimebank.Checked += (sender, args) => { Config.AutoclickTimebank = true; };
            CheckBox_AutoclickTimebank.Unchecked += (sender, args) => { Config.AutoclickTimebank = false; };
            CheckBox_AutoclickYesSeatAvailable.Checked += (sender, args) => { Config.AutoclickYesSeatAvailable = true; };
            CheckBox_AutoclickYesSeatAvailable.Unchecked += (sender, args) => { Config.AutoclickYesSeatAvailable = false; };
            CheckBox_AutocloseTournamentRegistrationPopups.Checked += (sender, args) => { Config.AutocloseTournamentRegistrationPopups = true; };
            CheckBox_AutocloseTournamentRegistrationPopups.Unchecked += (sender, args) => { Config.AutocloseTournamentRegistrationPopups = false; };
            CheckBox_AutocloseHm2ApplyToSimilarTablesPopups.Checked += (sender, args) => { Config.AutocloseHM2ApplyToSimilarTablesPopups = true; };
            CheckBox_AutocloseHm2ApplyToSimilarTablesPopups.Unchecked += (sender, args) => { Config.AutocloseHM2ApplyToSimilarTablesPopups = false; };

            TextBoxHotkey_HandReplay.TextChanged += (sender, args) => { Config.HotkeyHandReplay = TextBoxHotkey_HandReplay.KeyCombination; };
            TextBoxHotkey_QuickPreview.TextChanged += (sender, args) => { Config.HotkeyQuickPreview = TextBoxHotkey_QuickPreview.KeyCombination; };

            // Seed

            CheckBox_AutoclickImBack.IsChecked = Config.AutoclickImBack;
            CheckBox_AutoclickTimebank.IsChecked = Config.AutoclickTimebank;
            CheckBox_AutoclickYesSeatAvailable.IsChecked = Config.AutoclickYesSeatAvailable;
            CheckBox_AutocloseTournamentRegistrationPopups.IsChecked = Config.AutocloseTournamentRegistrationPopups;
            CheckBox_AutocloseHm2ApplyToSimilarTablesPopups.IsChecked = Config.AutocloseHM2ApplyToSimilarTablesPopups;
            TextBoxHotkey_HandReplay.KeyCombination = Config.HotkeyHandReplay;
            TextBoxHotkey_QuickPreview.RestrictedToSingeKeys = true;
            TextBoxHotkey_QuickPreview.KeyCombination = Config.HotkeyQuickPreview;

            // ToolTips

            CheckBox_AutoclickImBack.ToolTip = new UCToolTipControllerImBack();
            ToolTipService.SetShowDuration(CheckBox_AutoclickImBack, 60000);

            CheckBox_AutoclickTimebank.ToolTip = new UCToolTipControllerTimebank();
            ToolTipService.SetShowDuration(CheckBox_AutoclickTimebank, 60000);

            CheckBox_AutoclickYesSeatAvailable.ToolTip = new UCToolTipControllerSeatAvailable();
            ToolTipService.SetShowDuration(CheckBox_AutoclickYesSeatAvailable, 60000);

            CheckBox_AutocloseTournamentRegistrationPopups.ToolTip = new UCToolTipControllerTournamentRegistration();
            ToolTipService.SetShowDuration(CheckBox_AutocloseTournamentRegistrationPopups, 60000);

            CheckBox_AutocloseHm2ApplyToSimilarTablesPopups.ToolTip = new UCToolTipControllerHm2ApplyToSimilar();
            ToolTipService.SetShowDuration(CheckBox_AutocloseHm2ApplyToSimilarTablesPopups, 60000);

            Label_HandReplayHotkey.ToolTip = new UCToolTipControllerHandReplayHotkey();
            ToolTipService.SetShowDuration(Label_HandReplayHotkey, 60000);

            Label_QuickPreviewHotkey.ToolTip = new UCToolTipControllerQuickPreview();
            ToolTipService.SetShowDuration(Label_QuickPreviewHotkey, 60000);
        }

        private void Button_OpenHandReplayer_Click(object sender, RoutedEventArgs e)
        {
            App.WindowReplayer.Visibility = Visibility.Visible;
        }
    }
}

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
            CheckBox_AutoclickImBackDisableDuringBreaks.Checked += (sender, args) => { Config.AutoclickImBackDisableDuringBreaks = true; };
            CheckBox_AutoclickImBackDisableDuringBreaks.Unchecked += (sender, args) => { Config.AutoclickImBackDisableDuringBreaks = false; };
            CheckBox_AutoclickTimebank.Checked += (sender, args) => { Config.AutoclickTimebank = true; };
            CheckBox_AutoclickTimebank.Unchecked += (sender, args) => { Config.AutoclickTimebank = false; };
            CheckBox_AutoclickYesSeatAvailable.Checked += (sender, args) => { Config.AutoclickYesSeatAvailable = true; };
            CheckBox_AutoclickYesSeatAvailable.Unchecked += (sender, args) => { Config.AutoclickYesSeatAvailable = false; };
            CheckBox_AutocloseTournamentRegistrationPopups.Checked += (sender, args) => { Config.AutocloseTournamentRegistrationPopups = true; };
            CheckBox_AutocloseTournamentRegistrationPopups.Unchecked += (sender, args) => { Config.AutocloseTournamentRegistrationPopups = false; };
            CheckBox_AutocloseHm2ApplyToSimilarTablesPopups.Checked += (sender, args) => { Config.AutocloseHM2ApplyToSimilarTablesPopups = true; };
            CheckBox_AutocloseHm2ApplyToSimilarTablesPopups.Unchecked += (sender, args) => { Config.AutocloseHM2ApplyToSimilarTablesPopups = false; };

            CheckBox_AutoclickImBack.Checked += (sender, args) => CheckBox_AutoclickImBackDisableDuringBreaks.IsEnabled = true;
            CheckBox_AutoclickImBack.Unchecked += (sender, args) => CheckBox_AutoclickImBackDisableDuringBreaks.IsEnabled = false;

            CheckBox_EnableCustomTablesWindowStyle.Checked += (sender, args) =>
            {
                Config.EnableCustomTablesWindowStyle = true;
                RadioButton_NoCaption.IsEnabled = true;
                RadioButton_Borderless.IsEnabled = true;
                Label_DisabledNoCaption.Visibility = Visibility.Collapsed;
                Label_DisabledBorderless.Visibility = Visibility.Collapsed;
                App.TableManager.EnsureTablesStyle();
            };
            CheckBox_EnableCustomTablesWindowStyle.Unchecked += (sender, args) =>
            {
                Config.EnableCustomTablesWindowStyle = false;
                RadioButton_NoCaption.IsEnabled = false;
                RadioButton_Borderless.IsEnabled = false;
                Label_DisabledNoCaption.Visibility = Visibility.Visible;
                Label_DisabledBorderless.Visibility = Visibility.Visible;
                App.TableManager.EnsureTablesStyle();
            };

            RadioButton_NoCaption.Checked += (sender, args) =>
            {
                Config.CustomTablesWindowStyle = Config.TableWindowStyle.NoCaption;
                App.TableManager.EnsureTablesStyle();
            };
            RadioButton_Borderless.Checked += (sender, args) =>
            {
                Config.CustomTablesWindowStyle = Config.TableWindowStyle.Borderless;
                App.TableManager.EnsureTablesStyle();
            };

            TextBoxHotkey_HandReplay.TextChanged += (sender, args) => { Config.HotkeyHandReplay = TextBoxHotkey_HandReplay.KeyCombination; };
            TextBoxHotkey_QuickPreview.TextChanged += (sender, args) => { Config.HotkeyQuickPreview = TextBoxHotkey_QuickPreview.KeyCombination; };

            // Seed

            CheckBox_AutoclickImBack.IsChecked = Config.AutoclickImBack;
            CheckBox_AutoclickImBackDisableDuringBreaks.IsChecked = Config.AutoclickImBackDisableDuringBreaks;
            CheckBox_AutoclickTimebank.IsChecked = Config.AutoclickTimebank;
            CheckBox_AutoclickYesSeatAvailable.IsChecked = Config.AutoclickYesSeatAvailable;
            CheckBox_AutocloseTournamentRegistrationPopups.IsChecked = Config.AutocloseTournamentRegistrationPopups;
            CheckBox_AutocloseHm2ApplyToSimilarTablesPopups.IsChecked = Config.AutocloseHM2ApplyToSimilarTablesPopups;
            TextBoxHotkey_HandReplay.KeyCombination = Config.HotkeyHandReplay;
            TextBoxHotkey_QuickPreview.RestrictedToSingeKeys = true;
            TextBoxHotkey_QuickPreview.KeyCombination = Config.HotkeyQuickPreview;

            CheckBox_EnableCustomTablesWindowStyle.IsChecked = Config.EnableCustomTablesWindowStyle;
            switch (Config.CustomTablesWindowStyle)
            {
                case Config.TableWindowStyle.NoCaption:
                    RadioButton_NoCaption.IsChecked = true;
                    RadioButton_Borderless.IsChecked = false;
                    break;
                case Config.TableWindowStyle.Borderless:
                    RadioButton_NoCaption.IsChecked = false;
                    RadioButton_Borderless.IsChecked = true;
                    break;
            }

            // ToolTips

            CheckBox_AutoclickImBack.ToolTip = new UCToolTipControllerImBack();
            ToolTipService.SetShowDuration(CheckBox_AutoclickImBack, 60000);

            CheckBox_AutoclickImBackDisableDuringBreaks.ToolTip = "During final table breaks when there are few people left, \"I'm Ready\" button might appear.\n" +
                                                                  "It is similar to \"I'm Back\" button and might be clicked. This is due to method used for button recognition (average color matching).";
            ToolTipService.SetShowDuration(CheckBox_AutoclickImBackDisableDuringBreaks, 60000);

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

            RadioButton_NoCaption.ToolTip = new UCToolTipControllerWindowStyleNoCaption();
            ToolTipService.SetShowDuration(RadioButton_NoCaption, 60000);

            RadioButton_Borderless.ToolTip = new UCToolTipControllerWindowStyleBorderless();
            ToolTipService.SetShowDuration(RadioButton_Borderless, 60000);

            Label_DisabledNoCaption.ToolTip = new UCToolTipControllerWindowStyleNoCaption();
            ToolTipService.SetShowDuration(Label_DisabledNoCaption, 60000);

            Label_DisabledBorderless.ToolTip = new UCToolTipControllerWindowStyleBorderless();
            ToolTipService.SetShowDuration(Label_DisabledBorderless, 60000);
        }
    }
}

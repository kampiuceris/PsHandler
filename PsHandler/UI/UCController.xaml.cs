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
            CheckBox_AutocloseHM2ApplyToSimilarTablesPopups.Checked += (sender, args) => { Config.AutocloseHM2ApplyToSimilarTablesPopups = true; };
            CheckBox_AutocloseHM2ApplyToSimilarTablesPopups.Unchecked += (sender, args) => { Config.AutocloseHM2ApplyToSimilarTablesPopups = false; };

            CheckBox_EnableCustomTablesWindowStyle.Checked += (sender, args) =>
            {
                Config.EnableCustomTablesWindowStyle = true;
                RadioButton_NoCaption.IsEnabled = true;
                RadioButton_Borderless.IsEnabled = true;
                App.TableManager.EnsureTablesStyle();
            };
            CheckBox_EnableCustomTablesWindowStyle.Unchecked += (sender, args) =>
            {
                Config.EnableCustomTablesWindowStyle = false;
                RadioButton_NoCaption.IsEnabled = false;
                RadioButton_Borderless.IsEnabled = false;
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
            CheckBox_AutoclickTimebank.IsChecked = Config.AutoclickTimebank;
            CheckBox_AutoclickYesSeatAvailable.IsChecked = Config.AutoclickYesSeatAvailable;
            CheckBox_AutocloseTournamentRegistrationPopups.IsChecked = Config.AutocloseTournamentRegistrationPopups;
            CheckBox_AutocloseHM2ApplyToSimilarTablesPopups.IsChecked = Config.AutocloseHM2ApplyToSimilarTablesPopups;
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
        }
    }
}

using System.Windows.Controls;
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

            // Init values

            CheckBox_AutoclickImBack.IsChecked = Config.AutoclickImBack;

            CheckBox_AutoclickTimebank.IsChecked = Config.AutoclickTimebank;

            CheckBox_AutoclickYesSeatAvailable.IsChecked = Config.AutoclickYesSeatAvailable;

            CheckBox_AutocloseTournamentRegistrationPopups.IsChecked = Config.AutocloseTournamentRegistrationPopups;

            CheckBox_AutocloseHM2ApplyToSimilarTablesPopups.IsChecked = Config.AutocloseHM2ApplyToSimilarTablesPopups;

            TextBoxHotkey_HandReplay.KeyCombination = Config.HotkeyHandReplay;

            // Hook values

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

            TextBoxHotkey_HandReplay.TextChanged += (sender, args) => { Config.HotkeyHandReplay = TextBoxHotkey_HandReplay.KeyCombination; };

            // ToolTips

            Image_AutoclickImBack.ToolTip = new UCToolTipButtonImBack();
            Image_AutoclickTimebank.ToolTip = new UCToolTipButtonTimebank();
            Image_AutoclickYesWaitingList.ToolTip = new UCToolTipAutoclickYesSeatAvailable();
            Image_AutoclickTournamentRegistrationPopups.ToolTip = new UCToolTipAutocloseTournamentRegistrationPopups();
            Image_AutoclickHM2ApplyToSimilarTablesPopup.ToolTip = new UCToolTipAutocloseHM2ApplyToSimilarTablesPopups();
        }
    }
}

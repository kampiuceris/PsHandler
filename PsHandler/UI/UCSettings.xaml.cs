using System.Windows.Controls;

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

            TextBox_PokerStarsAppDataFolderPath.Text = Config.AppDataPath;

            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Unknown());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Azure());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Black());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Classic());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.HyperSimple());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Nova());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Slick());
            ComboBox_PokerStarsThemeTable.Items.Add(new PokerStarsThemesTable.Stars());
            foreach (var item in ComboBox_PokerStarsThemeTable.Items)
            {
                if (Config.PokerStarsThemeTable.GetType() == item.GetType())
                {
                    ComboBox_PokerStarsThemeTable.SelectedItem = item;
                    break;
                }
            }

            CheckBox_MinimizeToSystemTray.IsChecked = Config.MinimizeToSystemTray;

            CheckBox_StartMinimized.IsChecked = Config.StartMinimized;

            TextBoxHotkey_Exit.KeyCombination = Config.HotkeyExit;

            CheckBox_SaveGuiLocation.IsChecked = Config.SaveGuiLocation;

            CheckBox_SaveGuiSize.IsChecked = Config.SaveGuiSize;

            // Hook values

            TextBox_PokerStarsAppDataFolderPath.TextChanged += (sender, args) => { Config.AppDataPath = TextBox_PokerStarsAppDataFolderPath.Text; };

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
        }
    }
}

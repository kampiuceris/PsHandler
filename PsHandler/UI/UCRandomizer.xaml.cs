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

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCRandomizer.xaml
    /// </summary>
    public partial class UCRandomizer : UserControl
    {
        public UCRandomizer()
        {
            InitializeComponent();

            // Init values

            CheckBox_EnableRandomizer.IsChecked = Config.EnableRandomizer;

            TextBoxHotkey_Chance10.KeyCombination = Config.HotkeyRandomizerChance10;
            TextBoxHotkey_Chance20.KeyCombination = Config.HotkeyRandomizerChance20;
            TextBoxHotkey_Chance30.KeyCombination = Config.HotkeyRandomizerChance30;
            TextBoxHotkey_Chance40.KeyCombination = Config.HotkeyRandomizerChance40;
            TextBoxHotkey_Chance50.KeyCombination = Config.HotkeyRandomizerChance50;
            TextBoxHotkey_Chance60.KeyCombination = Config.HotkeyRandomizerChance60;
            TextBoxHotkey_Chance70.KeyCombination = Config.HotkeyRandomizerChance70;
            TextBoxHotkey_Chance80.KeyCombination = Config.HotkeyRandomizerChance80;
            TextBoxHotkey_Chance90.KeyCombination = Config.HotkeyRandomizerChance90;

            TextBox_Chance10.Text = Config.RandomizerChance10.ToString(CultureInfo.InvariantCulture);
            TextBox_Chance20.Text = Config.RandomizerChance20.ToString(CultureInfo.InvariantCulture);
            TextBox_Chance30.Text = Config.RandomizerChance30.ToString(CultureInfo.InvariantCulture);
            TextBox_Chance40.Text = Config.RandomizerChance40.ToString(CultureInfo.InvariantCulture);
            TextBox_Chance50.Text = Config.RandomizerChance50.ToString(CultureInfo.InvariantCulture);
            TextBox_Chance60.Text = Config.RandomizerChance60.ToString(CultureInfo.InvariantCulture);
            TextBox_Chance70.Text = Config.RandomizerChance70.ToString(CultureInfo.InvariantCulture);
            TextBox_Chance80.Text = Config.RandomizerChance80.ToString(CultureInfo.InvariantCulture);
            TextBox_Chance90.Text = Config.RandomizerChance90.ToString(CultureInfo.InvariantCulture);

            // Hook

            CheckBox_EnableRandomizer.Checked += (sender, args) => { Config.EnableRandomizer = true; };
            CheckBox_EnableRandomizer.Unchecked += (sender, args) => { Config.EnableRandomizer = false; };

            TextBoxHotkey_Chance10.TextChanged += (sender, args) => { Config.HotkeyRandomizerChance10 = TextBoxHotkey_Chance10.KeyCombination; };
            TextBoxHotkey_Chance20.TextChanged += (sender, args) => { Config.HotkeyRandomizerChance20 = TextBoxHotkey_Chance20.KeyCombination; };
            TextBoxHotkey_Chance30.TextChanged += (sender, args) => { Config.HotkeyRandomizerChance30 = TextBoxHotkey_Chance30.KeyCombination; };
            TextBoxHotkey_Chance40.TextChanged += (sender, args) => { Config.HotkeyRandomizerChance40 = TextBoxHotkey_Chance40.KeyCombination; };
            TextBoxHotkey_Chance50.TextChanged += (sender, args) => { Config.HotkeyRandomizerChance50 = TextBoxHotkey_Chance50.KeyCombination; };
            TextBoxHotkey_Chance60.TextChanged += (sender, args) => { Config.HotkeyRandomizerChance60 = TextBoxHotkey_Chance60.KeyCombination; };
            TextBoxHotkey_Chance70.TextChanged += (sender, args) => { Config.HotkeyRandomizerChance70 = TextBoxHotkey_Chance70.KeyCombination; };
            TextBoxHotkey_Chance80.TextChanged += (sender, args) => { Config.HotkeyRandomizerChance80 = TextBoxHotkey_Chance80.KeyCombination; };
            TextBoxHotkey_Chance90.TextChanged += (sender, args) => { Config.HotkeyRandomizerChance90 = TextBoxHotkey_Chance90.KeyCombination; };

            TextBox_Chance10.TextChanged += (sender, args) =>
            {
                int.TryParse(TextBox_Chance10.Text, out Config.RandomizerChance10);
                if (Config.RandomizerChance10 < 0) Config.RandomizerChance10 = 0;
                if (Config.RandomizerChance10 > 100) Config.RandomizerChance10 = 100;
            };
            TextBox_Chance20.TextChanged += (sender, args) =>
            {
                int.TryParse(TextBox_Chance20.Text, out Config.RandomizerChance20);
                if (Config.RandomizerChance20 < 0) Config.RandomizerChance20 = 0;
                if (Config.RandomizerChance20 > 100) Config.RandomizerChance20 = 100;
            };
            TextBox_Chance30.TextChanged += (sender, args) =>
            {
                int.TryParse(TextBox_Chance30.Text, out Config.RandomizerChance30);
                if (Config.RandomizerChance30 < 0) Config.RandomizerChance30 = 0;
                if (Config.RandomizerChance30 > 100) Config.RandomizerChance30 = 100;
            };
            TextBox_Chance40.TextChanged += (sender, args) =>
            {
                int.TryParse(TextBox_Chance40.Text, out Config.RandomizerChance40);
                if (Config.RandomizerChance40 < 0) Config.RandomizerChance40 = 0;
                if (Config.RandomizerChance40 > 100) Config.RandomizerChance40 = 100;
            };
            TextBox_Chance50.TextChanged += (sender, args) =>
            {
                int.TryParse(TextBox_Chance50.Text, out Config.RandomizerChance50);
                if (Config.RandomizerChance50 < 0) Config.RandomizerChance50 = 0;
                if (Config.RandomizerChance50 > 100) Config.RandomizerChance50 = 100;
            };
            TextBox_Chance60.TextChanged += (sender, args) =>
            {
                int.TryParse(TextBox_Chance60.Text, out Config.RandomizerChance60);
                if (Config.RandomizerChance60 < 0) Config.RandomizerChance60 = 0;
                if (Config.RandomizerChance60 > 100) Config.RandomizerChance60 = 100;
            };
            TextBox_Chance70.TextChanged += (sender, args) =>
            {
                int.TryParse(TextBox_Chance70.Text, out Config.RandomizerChance70);
                if (Config.RandomizerChance70 < 0) Config.RandomizerChance70 = 0;
                if (Config.RandomizerChance70 > 100) Config.RandomizerChance70 = 100;
            };
            TextBox_Chance80.TextChanged += (sender, args) =>
            {
                int.TryParse(TextBox_Chance80.Text, out Config.RandomizerChance80);
                if (Config.RandomizerChance80 < 0) Config.RandomizerChance80 = 0;
                if (Config.RandomizerChance80 > 100) Config.RandomizerChance80 = 100;
            };
            TextBox_Chance90.TextChanged += (sender, args) =>
            {
                int.TryParse(TextBox_Chance90.Text, out Config.RandomizerChance90);
                if (Config.RandomizerChance90 < 0) Config.RandomizerChance90 = 0;
                if (Config.RandomizerChance90 > 100) Config.RandomizerChance90 = 100;
            };
        }
    }
}

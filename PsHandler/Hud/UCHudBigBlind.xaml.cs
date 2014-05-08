using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsHandler.Hud
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

            TextBox_BigBlindDecimals.Text = Config.BigBlindDecimals.ToString(CultureInfo.InvariantCulture);

            CheckBox_EnableHudBigBlind.IsChecked = HudManager.EnableHudBigBlind;

            // Hook values

            TextBox_BigBlindDecimals.TextChanged += (sender, args) =>
            {
                int i;
                if (int.TryParse(TextBox_BigBlindDecimals.Text, out i))
                {
                    if (i < 0) i = 0;
                    if (i > 4) i = 4;
                }
                Config.BigBlindDecimals = i;
            };

            CheckBox_EnableHudBigBlind.Checked += (sender, args) => { HudManager.EnableHudBigBlind = true; };
            CheckBox_EnableHudBigBlind.Unchecked += (sender, args) => { HudManager.EnableHudBigBlind = false; };
        }

        private void Button_CustomizeBigBlind_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WindowCustomizeHud(
                App.WindowMain,
                HudManager.BigBlindHudBackground,
                HudManager.BigBlindHudForeground,
                HudManager.BigBlindHudFontFamily,
                HudManager.BigBlindHudFontWeight,
                HudManager.BigBlindHudFontStyle,
                HudManager.BigBlindHudFontSize,
                "15.7");
            dialog.ShowDialog();
            if (dialog.Saved)
            {
                HudManager.BigBlindHudBackground = dialog.HudBackground;
                HudManager.BigBlindHudForeground = dialog.HudForeground;
                HudManager.BigBlindHudFontFamily = dialog.HudFontFamily;
                HudManager.BigBlindHudFontWeight = dialog.HudFontWeight;
                HudManager.BigBlindHudFontStyle = dialog.HudFontStyle;
                HudManager.BigBlindHudFontSize = dialog.HudFontSize;
            }
        }
    }
}

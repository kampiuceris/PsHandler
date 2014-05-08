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
using PsHandler.PokerTypes;

namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for UCHUDTimer.xaml
    /// </summary>
    public partial class UCHudTimer : UserControl
    {
        public UCHudTimer()
        {
            InitializeComponent();

            // Init values

            TextBox_TimerDiff.Text = Config.TimeDiff.ToString(CultureInfo.InvariantCulture);

            CheckBox_EnableHudTimer.IsChecked = HudManager.EnableHudTimer;

            // Hook values

            TextBox_TimerDiff.TextChanged += (sender, args) => int.TryParse(TextBox_TimerDiff.Text, out Config.TimeDiff);

            CheckBox_EnableHudTimer.Checked += (sender, args) => { HudManager.EnableHudTimer = true; };
            CheckBox_EnableHudTimer.Unchecked += (sender, args) => { HudManager.EnableHudTimer = false; };
        }

        private void Button_CustomizeTimer_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WindowCustomizeHud(
                App.WindowMain,
                HudManager.TimerHudBackground,
                HudManager.TimerHudForeground,
                HudManager.TimerHudFontFamily,
                HudManager.TimerHudFontWeight,
                HudManager.TimerHudFontStyle,
                HudManager.TimerHudFontSize,
                "01:23");
            dialog.ShowDialog();
            if (dialog.Saved)
            {
                HudManager.TimerHudBackground = dialog.HudBackground;
                HudManager.TimerHudForeground = dialog.HudForeground;
                HudManager.TimerHudFontFamily = dialog.HudFontFamily;
                HudManager.TimerHudFontWeight = dialog.HudFontWeight;
                HudManager.TimerHudFontStyle = dialog.HudFontStyle;
                HudManager.TimerHudFontSize = dialog.HudFontSize;
            }
        }
    }
}

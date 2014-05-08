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
    /// Interaction logic for UCHudTimer.xaml
    /// </summary>
    public partial class UCHudTimer : UserControl
    {
        public UCHudTimer()
        {
            InitializeComponent();

            // Init values

            CheckBox_EnableHudTimer.IsChecked = HudManager.EnableHudTimer;
            TextBox_TimerDiff.Text = Config.TimerDiff.ToString(CultureInfo.InvariantCulture);
            TextBox_TimerHHNotFound.Text = Config.TimerHHNotFound;
            TextBox_TimerPokerTypeNotFound.Text = Config.TimerPokerTypeNotFound;
            TextBox_TimerMultiplePokerTypes.Text = Config.TimerMultiplePokerTypes;
            TextBox_TimerLocationX.Text = HudManager.GetTimerHudLocationX(TextBox_TimerLocationX).ToString(CultureInfo.InvariantCulture);
            TextBox_TimerLocationY.Text = HudManager.GetTimerHudLocationY(TextBox_TimerLocationY).ToString(CultureInfo.InvariantCulture);

            // Hook values

            CheckBox_EnableHudTimer.Checked += (sender, args) => { HudManager.EnableHudTimer = true; };
            CheckBox_EnableHudTimer.Unchecked += (sender, args) => { HudManager.EnableHudTimer = false; };
            TextBox_TimerDiff.TextChanged += (sender, args) => int.TryParse(TextBox_TimerDiff.Text, out Config.TimerDiff);
            TextBox_TimerHHNotFound.TextChanged += (sender, args) => Config.TimerHHNotFound = TextBox_TimerHHNotFound.Text;
            TextBox_TimerPokerTypeNotFound.TextChanged += (sender, args) => Config.TimerPokerTypeNotFound = TextBox_TimerPokerTypeNotFound.Text;
            TextBox_TimerMultiplePokerTypes.TextChanged += (sender, args) => Config.TimerMultiplePokerTypes = TextBox_TimerMultiplePokerTypes.Text;
            CheckBox_LockTimerHudLocation.Checked += (sender, args) =>
            {
                HudManager.TimerHudLocationLocked = true;
                TextBox_TimerLocationX.IsEnabled = false;
                TextBox_TimerLocationY.IsEnabled = false;
            };
            CheckBox_LockTimerHudLocation.Unchecked += (sender, args) =>
            {
                HudManager.TimerHudLocationLocked = false;
                TextBox_TimerLocationX.IsEnabled = true;
                TextBox_TimerLocationY.IsEnabled = true;
            };
            TextBox_TimerLocationX.TextChanged += (sender, args) =>
            {
                float f;
                if (float.TryParse(TextBox_TimerLocationX.Text, out f))
                {
                    HudManager.SetTimerHudLocationX(f, TextBox_TimerLocationX);
                }
            };
            TextBox_TimerLocationY.TextChanged += (sender, args) =>
            {
                float f;
                if (float.TryParse(TextBox_TimerLocationY.Text, out f))
                {
                    HudManager.SetTimerHudLocationY(f, TextBox_TimerLocationY);
                }
            };

            // hook needed init values

            CheckBox_LockTimerHudLocation.IsChecked = HudManager.TimerHudLocationLocked;
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

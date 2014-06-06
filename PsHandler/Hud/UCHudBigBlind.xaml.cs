using PsHandler.Hud.Import;
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
        public readonly TextBox[] TextBox_BigBlindLocationX;
        public readonly TextBox[] TextBox_BigBlindLocationY;

        public UCHudBigBlind()
        {
            InitializeComponent();

            TextBox_BigBlindLocationX = new TextBox[]
            {
                TextBox_BigBlindLocationXDefault, 
                TextBox_BigBlindLocationX10max, 
                TextBox_BigBlindLocationX9max, 
                TextBox_BigBlindLocationX8max, 
                TextBox_BigBlindLocationX7max, 
                TextBox_BigBlindLocationX6max, 
                TextBox_BigBlindLocationX4max, 
                TextBox_BigBlindLocationX2max, 
            };
            TextBox_BigBlindLocationY = new TextBox[]
            {
                TextBox_BigBlindLocationYDefault, 
                TextBox_BigBlindLocationY10max, 
                TextBox_BigBlindLocationY9max, 
                TextBox_BigBlindLocationY8max, 
                TextBox_BigBlindLocationY7max, 
                TextBox_BigBlindLocationY6max, 
                TextBox_BigBlindLocationY4max, 
                TextBox_BigBlindLocationY2max, 
            };

            // Init values
            CheckBox_EnableHudBigBlind.IsChecked = HudManager.EnableHudBigBlind;
            TextBox_BigBlindDecimals.Text = Config.BigBlindDecimals.ToString(CultureInfo.InvariantCulture);

            for (int i = 0; i < TextBox_BigBlindLocationX.Length; i++)
            {
                TextBox_BigBlindLocationX[i].Text = HudManager.GetBigBlindHudLocationX((TableSize)i).ToString(CultureInfo.InvariantCulture);
                TextBox_BigBlindLocationY[i].Text = HudManager.GetBigBlindHudLocationY((TableSize)i).ToString(CultureInfo.InvariantCulture);
            }

            // Hook values

            CheckBox_EnableHudBigBlind.Checked += (sender, args) => { HudManager.EnableHudBigBlind = true; };
            CheckBox_EnableHudBigBlind.Unchecked += (sender, args) => { HudManager.EnableHudBigBlind = false; };
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
            CheckBox_LockBigBlindHudLocation.Checked += (sender, args) =>
            {
                HudManager.BigBlindHudLocationLocked = true;
                for (int i = 0; i < TextBox_BigBlindLocationX.Length; i++)
                {
                    TextBox_BigBlindLocationX[i].IsEnabled = false;
                    TextBox_BigBlindLocationY[i].IsEnabled = false;
                }
            };
            CheckBox_LockBigBlindHudLocation.Unchecked += (sender, args) =>
            {
                HudManager.BigBlindHudLocationLocked = false;
                for (int i = 0; i < TextBox_BigBlindLocationX.Length; i++)
                {
                    TextBox_BigBlindLocationX[i].IsEnabled = true;
                    TextBox_BigBlindLocationY[i].IsEnabled = true;
                }
            };
            for (int i = 0; i < TextBox_BigBlindLocationX.Length; i++)
            {
                int i1 = i;
                TextBox_BigBlindLocationX[i].TextChanged += (sender, args) =>
                {
                    float f;
                    if (float.TryParse(TextBox_BigBlindLocationX[i1].Text, out f))
                    {
                        HudManager.SetBigBlindHudLocationX((TableSize)i1, f, TextBox_BigBlindLocationX[i1]);
                    }
                };
                TextBox_BigBlindLocationY[i].TextChanged += (sender, args) =>
                {
                    float f;
                    if (float.TryParse(TextBox_BigBlindLocationY[i1].Text, out f))
                    {
                        HudManager.SetBigBlindHudLocationY((TableSize)i1, f, TextBox_BigBlindLocationY[i1]);
                    }
                };
            }

            // hook needed init values

            CheckBox_LockBigBlindHudLocation.IsChecked = HudManager.BigBlindHudLocationLocked;
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

        private void Button_CustomizeColorsByValue_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WindowCustomizeColorsByValue(App.WindowMain, HudManager.BigBlindHudForeground, HudManager.BigBlindColorsByValue);
            dialog.ShowDialog();
            if (dialog.Saved)
            {
                HudManager.BigBlindColorsByValue = dialog.ColorsByValue;
            }
        }
    }
}

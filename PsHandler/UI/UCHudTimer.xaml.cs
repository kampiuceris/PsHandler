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
using PsHandler.Hud;
using PsHandler.Import;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCHudTimer.xaml
    /// </summary>
    public partial class UCHudTimer : UserControl
    {
        public TextBox[] TextBoxesLocationX;
        public TextBox[] TextBoxesLocationY;

        public UCHudTimer()
        {
            InitializeComponent();

            TextBoxesLocationX = new List<TextBox>
            {
                TextBox_HudTimerLocationX_Default,
                TextBox_HudTimerLocationX_Max1,
                TextBox_HudTimerLocationX_Max2,
                TextBox_HudTimerLocationX_Max3,
                TextBox_HudTimerLocationX_Max4,
                TextBox_HudTimerLocationX_Max5,
                TextBox_HudTimerLocationX_Max6,
                TextBox_HudTimerLocationX_Max7,
                TextBox_HudTimerLocationX_Max8,
                TextBox_HudTimerLocationX_Max9,
                TextBox_HudTimerLocationX_Max10
            }.ToArray();
            TextBoxesLocationY = new List<TextBox>
            {
                TextBox_HudTimerLocationY_Default,
                TextBox_HudTimerLocationY_Max1,
                TextBox_HudTimerLocationY_Max2,
                TextBox_HudTimerLocationY_Max3,
                TextBox_HudTimerLocationY_Max4,
                TextBox_HudTimerLocationY_Max5,
                TextBox_HudTimerLocationY_Max6,
                TextBox_HudTimerLocationY_Max7,
                TextBox_HudTimerLocationY_Max8,
                TextBox_HudTimerLocationY_Max9,
                TextBox_HudTimerLocationY_Max10
            }.ToArray();

            // Init values

            CheckBox_EnableHudTimer.IsChecked = TableManager.EnableHudTimer;
            TextBox_TimerDiff.Text = Config.TimerDiff.ToString(CultureInfo.InvariantCulture);
            TextBox_TimerHHNotFound.Text = Config.TimerHHNotFound;
            TextBox_TimerPokerTypeNotFound.Text = Config.TimerPokerTypeNotFound;
            TextBox_TimerMultiplePokerTypes.Text = Config.TimerMultiplePokerTypes;

            for (int i = 0; i < TextBoxesLocationX.Length; i++)
            {
                TextBoxesLocationX[i].Text = TableManager.GetHudTimerLocationX((TableSize)i, TextBoxesLocationX[i]).ToString(CultureInfo.InvariantCulture);
                TextBoxesLocationY[i].Text = TableManager.GetHudTimerLocationY((TableSize)i, TextBoxesLocationY[i]).ToString(CultureInfo.InvariantCulture);
            }

            // Hook values

            CheckBox_EnableHudTimer.Checked += (sender, args) => { TableManager.EnableHudTimer = true; };
            CheckBox_EnableHudTimer.Unchecked += (sender, args) => { TableManager.EnableHudTimer = false; };
            TextBox_TimerDiff.TextChanged += (sender, args) => int.TryParse(TextBox_TimerDiff.Text, out Config.TimerDiff);
            TextBox_TimerHHNotFound.TextChanged += (sender, args) => Config.TimerHHNotFound = TextBox_TimerHHNotFound.Text;
            TextBox_TimerPokerTypeNotFound.TextChanged += (sender, args) => Config.TimerPokerTypeNotFound = TextBox_TimerPokerTypeNotFound.Text;
            TextBox_TimerMultiplePokerTypes.TextChanged += (sender, args) => Config.TimerMultiplePokerTypes = TextBox_TimerMultiplePokerTypes.Text;

            CheckBox_LockHudTimerLocation.Checked += (sender, args) =>
            {
                TableManager.HudTimerLocationLocked = true;
                for (int i = 0; i < TextBoxesLocationX.Length; i++)
                {
                    TextBoxesLocationX[i].IsEnabled = false;
                    TextBoxesLocationY[i].IsEnabled = false;
                }
            };
            CheckBox_LockHudTimerLocation.Unchecked += (sender, args) =>
            {
                TableManager.HudTimerLocationLocked = false;
                for (int i = 0; i < TextBoxesLocationX.Length; i++)
                {
                    TextBoxesLocationX[i].IsEnabled = true;
                    TextBoxesLocationY[i].IsEnabled = true;
                }
            };

            for (int i = 0; i < TextBoxesLocationX.Length; i++)
            {
                int i1 = i;
                TextBoxesLocationX[i].TextChanged += (sender, args) =>
                {
                    float f;
                    if (float.TryParse(TextBoxesLocationX[i1].Text, out f))
                    {
                        TableManager.SetHudTimerLocationX((TableSize)i1, f, TextBoxesLocationX[i1]);
                    }
                };
                TextBoxesLocationY[i].TextChanged += (sender, args) =>
                {
                    float f;
                    if (float.TryParse(TextBoxesLocationY[i1].Text, out f))
                    {
                        TableManager.SetHudTimerLocationY((TableSize)i1, f, TextBoxesLocationY[i1]);
                    }
                };
            }

            // hook needed init values

            CheckBox_LockHudTimerLocation.IsChecked = TableManager.HudTimerLocationLocked;

            // ToolTips //TODO
        }

        private void Button_Customize_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WindowHudDesign(
                App.WindowMain,
                Config.HudTimerBackground,
                Config.HudTimerForeground,
                Config.HudTimerFontFamily,
                Config.HudTimerFontWeight,
                Config.HudTimerFontStyle,
                Config.HudTimerFontSize,
                Config.HudTimerMargin,
                "01:23");
            dialog.ShowDialog();
            if (dialog.Saved)
            {
                Config.HudTimerBackground = dialog.HudBackground;
                Config.HudTimerForeground = dialog.HudForeground;
                Config.HudTimerFontFamily = dialog.HudFontFamily;
                Config.HudTimerFontWeight = dialog.HudFontWeight;
                Config.HudTimerFontStyle = dialog.HudFontStyle;
                Config.HudTimerFontSize = dialog.HudFontSize;
                Config.HudTimerMargin = dialog.HudMargin;
            }
        }
    }
}

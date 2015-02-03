using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PsHandler.PokerMath;

namespace PsHandler.Replayer.UI
{
    /// <summary>
    /// Interaction logic for WindowReplayer.xaml
    /// </summary>
    public partial class WindowReplayer : Window
    {
        public WindowReplayer()
        {
            InitializeComponent();

            KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Left)
                {
                    UcReplayerTable_Main.UndoCommand();
                }
                if (args.Key == Key.Right)
                {
                    //UcReplayerTable_Main.DoCommand();
                }
                if (args.Key == Key.Up)
                {
                    //UcReplayerTable_Main.ReplayHand(PokerHand.FromHandHistory(Clipboard.GetText()));
                }
            };

            MouseDown += (sender, args) =>
            {
                Keyboard.ClearFocus();
            };

            PreviewKeyDown += (sender, args) =>
            {
                if (TextBox_Payouts.IsFocused)
                {

                }
                else
                {
                    args.Handled = true;
                }
            };

            ComboBox_Payouts.Items.Add("Custom ...");
            ComboBox_Payouts.Items.Add("Heads up");
            ComboBox_Payouts.Items.Add("6 man");
            ComboBox_Payouts.Items.Add("9 man");
            ComboBox_Payouts.Items.Add("Fifty50");
            ComboBox_Payouts.Items.Add("18 man FT");
            ComboBox_Payouts.Items.Add("45 man FT");
            ComboBox_Payouts.Items.Add("90 man FT");
            ComboBox_Payouts.Items.Add("180 man FT");
            ComboBox_Payouts.Items.Add("360 man FT");
        }

        private void Button_Last_Click(object sender, RoutedEventArgs e)
        {
            UcReplayerTable_Main.DoCommandAll();
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            UcReplayerTable_Main.DoCommand();
        }

        private void Button_Previous_Click(object sender, RoutedEventArgs e)
        {
            UcReplayerTable_Main.UndoCommand();
        }

        private void Button_First_Click(object sender, RoutedEventArgs e)
        {
            UcReplayerTable_Main.UndoCommandAll();
        }

        private void Button_HandHistory_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_CalculateEv_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_PasteFromClipboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_GoToPreflop_Click(object sender, RoutedEventArgs e)
        {
            UcReplayerTable_Main.GoToPreflop();
        }

        private void Button_GoToFlop_Click(object sender, RoutedEventArgs e)
        {
            UcReplayerTable_Main.GoToFlop();
        }

        private void Button_GoToTurn_Click(object sender, RoutedEventArgs e)
        {
            UcReplayerTable_Main.GoToTurn();
        }

        private void Button_GoToRiver_Click(object sender, RoutedEventArgs e)
        {
            UcReplayerTable_Main.GoToRiver();
        }
    }
}

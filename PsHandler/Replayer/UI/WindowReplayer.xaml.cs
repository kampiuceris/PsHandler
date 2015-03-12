// PsHandler - poker software helping tool.
// Copyright (C) 2014-2015  kampiuceris

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PsHandler.Custom;
using PsHandler.PokerMath;
using PsHandler.UI;

namespace PsHandler.Replayer.UI
{
    /// <summary>
    /// Interaction logic for WindowReplayer.xaml
    /// </summary>
    public partial class WindowReplayer : Window
    {
        public bool IsClosing;

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

            InitPayouts();

            TextBox_Payouts.TextChanged += (sender, args) =>
            {
                var payouts = GetPayouts();
                if (payouts == null)
                {
                    TextBox_Payouts.Foreground = Brushes.Red;
                    TextBox_Payouts.ToolTip = "Error";
                }
                else
                {
                    TextBox_Payouts.Foreground = Brushes.Black;
                    TextBox_Payouts.ToolTip = payouts.Aggregate("", (a, b) => a + string.Format("{0:0.##########}{1}", b * 100, Environment.NewLine)).TrimEnd('\r', '\n');
                }
                ComboBox_Payouts.SelectedItem = null;
            };
            ComboBox_Payouts.SelectionChanged += (sender, args) =>
            {
                var selectedItem = ComboBox_Payouts.SelectedItem as ComboBoxItemPayouts;
                if (selectedItem != null)
                {
                    TextBox_Payouts.Text = selectedItem.Payouts.Aggregate("", (a, b) => a + string.Format("{0:0.##########} ", b * 100)).TrimEnd(' ');
                    TextBox_Payouts.ToolTip = selectedItem.ToolTip;
                }
            };

            Closing += (sender, args) =>
            {
                if (!IsClosing)
                {
                    Visibility = Visibility.Hidden;
                    args.Cancel = true;
                }
            };
        }

        private void InitPayouts()
        {
            //ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("Custom ...", new decimal[] { }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("Heads up", new[] { 1m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("6 man", new[] { 0.65m, 0.35m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("9 man", new[] { 0.5m, 0.3m, 0.2m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("Fifty50 10 man", new[] { 0.6m, 0.1m, 0.1m, 0.1m, 0.1m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("Fifty50 6 man", new[] { 0.8m, 0.1m, 0.1m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("18 man FT", new[] { 0.4m, 0.3m, 0.2m, 0.1m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("45 man FT", new[] { 0.31m, 0.215m, 0.165m, 0.125m, 0.09m, 0.06m, 0.035m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("90 man FT", new[] { 0.2755m, 0.185m, 0.14m, 0.092m, 0.067m, 0.0495m, 0.04m, 0.0335m, 0.0275m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("180 man FT", new[] { 0.3m, 0.2m, 0.114m, 0.074m, 0.058m, 0.043m, 0.03m, 0.022m, 0.015m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("360 man FT", new[] { 0.2045m, 0.15m, 0.113m, 0.085m, 0.058m, 0.045m, 0.035m, 0.025m, 0.019m }));
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
            var pokerHand = UcReplayerTable_Main.PokerHand;
            if (pokerHand != null)
            {
                WindowMessage.Show(pokerHand.HandHistory, "Hand History", WindowMessageButtons.OK, WindowMessageImage.None, this, WindowStartupLocation.CenterOwner, WindowMessageTextType.TextBox);
            }
        }

        private void Button_PasteFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UcReplayerTable_Main.ReplayHand(PokerHand.Parse(Methods.GetClipboardText()));
            }
            catch
            {
                UcReplayerTable_Main.ReplayHand(null);
            }
        }

        private void Button_CalculateEv_Click(object sender, RoutedEventArgs e)
        {
            var payouts = GetPayouts();
            if (payouts == null)
            {
                WindowMessage.ShowDialog("Invalid payouts.", "Error", WindowMessageButtons.OK, WindowMessageImage.Error, this);
                return;
            }

            var pokerHand = UcReplayerTable_Main.PokerHand;
            if (pokerHand != null)
            {
                var tempPokerHand = PokerHand.Parse(pokerHand.HandHistory);
                var tempEv = new Ev(tempPokerHand, payouts, tempPokerHand.BuyIn * (int)tempPokerHand.TableSize, tempPokerHand.Currency, PokerMath.Evaluator.Hand.Evaluate);
                WindowMessage.Show(tempEv.ToString(), "Expected Value Analysis", WindowMessageButtons.OK, WindowMessageImage.None, this, WindowStartupLocation.CenterOwner, WindowMessageTextType.TextBox, new FontFamily("Consolas"));
            }
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

        private decimal[] GetPayouts()
        {
            try
            {
                var split = TextBox_Payouts.Text.Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 0)
                {
                    return null;
                }

                var payouts = new decimal[split.Length];
                for (int i = 0; i < split.Length; i++)
                {
                    payouts[i] = decimal.Parse(split[i]);
                }
                var sum = payouts.Sum();
                for (int i = 0; i < payouts.Length; i++)
                {
                    payouts[i] /= sum;
                }

                return payouts;
            }
            catch
            {
                return null;
            }
        }

        internal class ComboBoxItemPayouts : ComboBoxItem
        {
            public string PayoutsName;
            public decimal[] Payouts;

            public ComboBoxItemPayouts(string payoutsName, decimal[] payouts)
            {
                PayoutsName = payoutsName;
                Payouts = payouts;
                Content = PayoutsName;
                ToolTip = Payouts.Aggregate("", (a, b) => a + string.Format("{0:0.##########}{1}", b * 100, Environment.NewLine)).TrimEnd('\r', '\n');
            }

            public override string ToString()
            {
                return PayoutsName;
            }
        }

        public void ReplayHand(string HandHistoryStr)
        {
            Visibility = Visibility.Visible;

            var pokerData = PokerData.FromText(HandHistoryStr);
            if (pokerData.PokerHands.Any())
            {
                UcReplayerTable_Main.ReplayHand(pokerData.PokerHands.First());
            }

            Topmost = true;
            Topmost = false;
        }
    }
}

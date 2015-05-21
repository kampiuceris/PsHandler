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
using System.Threading;
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
                    UcReplayerTable_Main.DoCommand();
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

            Closing += (sender, args) =>
            {
                if (!IsClosing)
                {
                    Visibility = Visibility.Hidden;
                    args.Cancel = true;
                }
            };
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
            new WindowCalculateEv(this, UcReplayerTable_Main.PokerHand.HandHistory).ShowDialog();
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

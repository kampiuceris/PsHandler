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

using System.Windows.Controls;
using System.Windows.Media;

namespace PsHandler.Replayer.UI
{
    /// <summary>
    /// Interaction logic for UcPlayer.xaml
    /// </summary>
    public partial class UcReplayerPlayer : UserControl
    {
        public UcReplayerPlayer()
        {
            InitializeComponent();
        }

        public void SetPlayerName(string playerName)
        {
            TextBlock_PlayerName.Text = playerName;
        }

        public void SetPlayerStack(decimal amount)
        {
            var amountStr = ReplayerBet.AmountToString(amount);
            TextBlock_PlayerStack.Text = (string.IsNullOrEmpty(amountStr) ? "0" : amountStr);
        }

        public void SetActionGlow(bool action)
        {
            Rectangle_PlayerBlock.Stroke = action ? Brushes.Gray : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF292929"));
        }
    }
}

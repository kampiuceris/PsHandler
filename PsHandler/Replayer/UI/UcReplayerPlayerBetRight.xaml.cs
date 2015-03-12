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

namespace PsHandler.Replayer.UI
{
    /// <summary>
    /// Interaction logic for UcPlayerBet.xaml
    /// </summary>
    public partial class UcReplayerPlayerBetRight : UserControl, IReplayerBet
    {
        private decimal _totalAmount;

        public UcReplayerPlayerBetRight()
        {
            InitializeComponent();
        }

        public void SetAmount(decimal totalAmount)
        {
            _totalAmount = totalAmount;
            ReplayerBet.SetAmount(_totalAmount, Grid_Chips, Label_Amount);
        }

        public decimal GetAmount()
        {
            return _totalAmount;
        }
    }
}

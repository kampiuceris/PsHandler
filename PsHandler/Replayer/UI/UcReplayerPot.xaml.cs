// PsHandler - poker software helping tool.
// Copyright (C) 2014  kampiuceris

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

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PsHandler.Replayer.UI
{
    /// <summary>
    /// Interaction logic for UcPot.xaml
    /// </summary>
    public partial class UcReplayerPot : UserControl, IReplayerBet
    {
        private decimal _totalAmount;

        public UcReplayerPot()
        {
            InitializeComponent();
        }

        public void SetAmount(decimal totalAmount)
        {
            _totalAmount = totalAmount;

            StackPanel_Chips.Children.Clear();

            List<List<Image>> chipsSorted = new List<List<Image>>();
            string temp = "";
            foreach (var image in ReplayerBet.GetChipImages(totalAmount))
            {
                var imageStr = image.Source.ToString();
                if (!imageStr.Equals(temp))
                {
                    chipsSorted.Add(new List<Image>());
                }
                chipsSorted.Last().Add(image);
                temp = imageStr;
            }

            foreach (var chips in chipsSorted)
            {
                Grid grid = new Grid();
                for (int i = 0; i < chips.Count; i++)
                {
                    var image = chips[i];
                    image.VerticalAlignment = VerticalAlignment.Bottom;
                    image.Margin = new Thickness(0, 0, 0, i * 4);
                    grid.Children.Add(image);
                }
                StackPanel_Chips.Children.Add(grid);
            }

            TextBlock_Amount.Text = ReplayerBet.AmountToString(totalAmount);
        }

        public decimal GetAmount()
        {
            return _totalAmount;
        }
    }
}

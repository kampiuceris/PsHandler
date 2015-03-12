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

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PsHandler.Replayer.UI
{
    interface IReplayerBet
    {
        void SetAmount(decimal totalAmount);
        decimal GetAmount();
    }

    public class ReplayerBet
    {
        public static Image GetImage(decimal value)
        {
            string valueStr;

            if (value == 0.01m) { valueStr = "chip000001"; }
            else if (value == 0.05m) { valueStr = "chip000005"; }
            else if (value == 0.25m) { valueStr = "chip000025"; }
            else if (value == 1) { valueStr = "chip0001"; }
            else if (value == 5) { valueStr = "chip0005"; }
            else if (value == 25) { valueStr = "chip0025"; }
            else if (value == 100) { valueStr = "chip0100"; }
            else if (value == 500) { valueStr = "chip0500"; }
            else if (value == 1000) { valueStr = "chip1000"; }
            else if (value == 5000) { valueStr = "chip5000"; }
            else if (value == 25000) { valueStr = "chip25000"; }
            else if (value == 100000) { valueStr = "chip100000"; }
            else if (value == 500000) { valueStr = "chip500000"; }
            else if (value == 1000000) { valueStr = "chip1000000"; }
            else if (value == 5000000) { valueStr = "chip5000000"; }
            else if (value == 25000000) { valueStr = "chip25000000"; }
            else if (value == 100000000) { valueStr = "chip100000000"; }
            else if (value == 500000000) { valueStr = "chip500000000"; }
            else if (value == 1000000000) { valueStr = "chip1000000000"; }
            else if (value == 5000000000) { valueStr = "chip5000000000"; }
            else if (value == 25000000000) { valueStr = "chip25000000000"; }
            else { valueStr = "chipone"; }

            return new Image
            {
                Source = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/Chips/{0}.png", valueStr), UriKind.Absolute)),
                Width = 22,
                Height = 20
            };
        }

        public static List<Image> GetChipImages(decimal totalAmount)
        {
            decimal amount = totalAmount;

            var chips = new List<Image>();
            while (amount > 0)
            {
                if (amount < 0.05m) { chips.Add(GetImage(0.01m)); amount -= 0.01m; continue; }
                if (amount < 0.25m) { chips.Add(GetImage(0.05m)); amount -= 0.05m; continue; }
                if (amount < 1) { chips.Add(GetImage(0.25m)); amount -= 0.25m; continue; }
                if (amount < 5) { chips.Add(GetImage(1)); amount -= 1; continue; }
                if (amount < 25) { chips.Add(GetImage(5)); amount -= 5; continue; }
                if (amount < 100) { chips.Add(GetImage(25)); amount -= 25; continue; }
                if (amount < 500) { chips.Add(GetImage(100)); amount -= 100; continue; }
                if (amount < 1000) { chips.Add(GetImage(500)); amount -= 500; continue; }
                if (amount < 5000) { chips.Add(GetImage(1000)); amount -= 1000; continue; }
                if (amount < 25000) { chips.Add(GetImage(5000)); amount -= 5000; continue; }
                if (amount < 100000) { chips.Add(GetImage(25000)); amount -= 25000; continue; }
                if (amount < 500000) { chips.Add(GetImage(100000)); amount -= 100000; continue; }
                if (amount < 1000000) { chips.Add(GetImage(500000)); amount -= 500000; continue; }
                if (amount < 5000000) { chips.Add(GetImage(1000000)); amount -= 1000000; continue; }
                if (amount < 25000000) { chips.Add(GetImage(5000000)); amount -= 5000000; continue; }
                if (amount < 100000000) { chips.Add(GetImage(25000000)); amount -= 25000000; continue; }
                if (amount < 500000000) { chips.Add(GetImage(100000000)); amount -= 100000000; continue; }
                if (amount < 1000000000) { chips.Add(GetImage(500000000)); amount -= 500000000; continue; }
                if (amount < 5000000000) { chips.Add(GetImage(1000000000)); amount -= 1000000000; continue; }
                if (amount < 25000000000) { chips.Add(GetImage(5000000000)); amount -= 5000000000; continue; }
                chips.Add(GetImage(25000000000)); amount -= 25000000000;
            }

            return chips;
        }

        public static void SetAmount(decimal totalAmount, Grid grid, Label label)
        {
            var chips = GetChipImages(totalAmount);

            grid.Children.Clear();

            for (int i = 0; i < chips.Count; i++)
            {
                var image = chips[i];
                image.VerticalAlignment = VerticalAlignment.Bottom;
                image.Margin = new Thickness(0, 0, 0, i * 4);
                grid.Children.Add(image);
            }

            label.Content = AmountToString(totalAmount);
            grid.ToolTip = label.Content;
            label.ToolTip = label.Content;
        }

        public static string AmountToString(decimal amount)
        {
            if (amount > 0)
            {
                if ((amount % 1) == 0)
                {
                    return string.Format("{0:#,##0}", amount);
                }
                else
                {
                    return string.Format("{0:#,##0.00}", amount);
                }
            }
            return "";
        }
    }
}

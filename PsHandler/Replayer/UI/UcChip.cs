using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using PsHandler.Custom;
using Image = System.Windows.Controls.Image;
using System.Windows;

namespace PsHandler.Replayer.UI
{
    public class UcChip : Image
    {
        public decimal Value;
        public int Size;

        public UcChip(decimal value, int size)
        {
            Value = value;
            Size = size;

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

            string uri = string.Format(@"/PsHandler;component/Images/Resources/Replayer/Chips/{0}/{1}.png", size, valueStr);
            Source = new BitmapImage(new Uri(uri, UriKind.Relative));
            Width = Converter.DEFAULT_CHIPS_SIZES[size].X;
            Height = Converter.DEFAULT_CHIPS_SIZES[size].Y;

            UseLayoutRounding = true;
        }

        public static List<UcChip> GetUcChips(decimal totalAmount, int size)
        {
            decimal amount = totalAmount;

            List<UcChip> chips = new List<UcChip>();
            while (amount > 0)
            {
                if (amount < 0.05m) { chips.Add(new UcChip(0.01m, size)); amount -= 0.01m; continue; }
                if (amount < 0.25m) { chips.Add(new UcChip(0.05m, size)); amount -= 0.05m; continue; }
                if (amount < 1) { chips.Add(new UcChip(0.25m, size)); amount -= 0.25m; continue; }
                if (amount < 5) { chips.Add(new UcChip(1, size)); amount -= 1; continue; }
                if (amount < 25) { chips.Add(new UcChip(5, size)); amount -= 5; continue; }
                if (amount < 100) { chips.Add(new UcChip(25, size)); amount -= 25; continue; }
                if (amount < 500) { chips.Add(new UcChip(100, size)); amount -= 100; continue; }
                if (amount < 1000) { chips.Add(new UcChip(500, size)); amount -= 500; continue; }
                if (amount < 5000) { chips.Add(new UcChip(1000, size)); amount -= 1000; continue; }
                if (amount < 25000) { chips.Add(new UcChip(5000, size)); amount -= 5000; continue; }
                if (amount < 100000) { chips.Add(new UcChip(25000, size)); amount -= 25000; continue; }
                if (amount < 500000) { chips.Add(new UcChip(100000, size)); amount -= 100000; continue; }
                if (amount < 1000000) { chips.Add(new UcChip(500000, size)); amount -= 500000; continue; }
                if (amount < 5000000) { chips.Add(new UcChip(1000000, size)); amount -= 1000000; continue; }
                if (amount < 25000000) { chips.Add(new UcChip(5000000, size)); amount -= 5000000; continue; }
                if (amount < 100000000) { chips.Add(new UcChip(25000000, size)); amount -= 25000000; continue; }
                if (amount < 500000000) { chips.Add(new UcChip(100000000, size)); amount -= 100000000; continue; }
                if (amount < 1000000000) { chips.Add(new UcChip(500000000, size)); amount -= 500000000; continue; }
                if (amount < 5000000000) { chips.Add(new UcChip(1000000000, size)); amount -= 1000000000; continue; }
                if (amount < 25000000000) { chips.Add(new UcChip(5000000000, size)); amount -= 5000000000; continue; }
                chips.Add(new UcChip(25000000000, size)); amount -= 25000000000;
            }

            return chips;
        }
    }
}

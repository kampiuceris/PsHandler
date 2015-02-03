using System;
using System.Collections.Generic;
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

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
    /// Interaction logic for UcPlayerBetLeft.xaml
    /// </summary>
    public partial class UcReplayerPlayerBetLeft : UserControl, IReplayerBet
    {
        private decimal _totalAmount;

        public UcReplayerPlayerBetLeft()
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

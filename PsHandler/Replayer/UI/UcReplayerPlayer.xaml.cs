using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PsHandler.PokerMath;

namespace PsHandler.Replayer.UI
{
    /// <summary>
    /// Interaction logic for WindowReplayer.xaml
    /// </summary>
    public partial class WindowReplayer : Window
    {
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
                    UcReplayerTable_Main.ReplayHand(PokerHand.FromHandHistory(Clipboard.GetText()));
                }
            };
        }
    }
}

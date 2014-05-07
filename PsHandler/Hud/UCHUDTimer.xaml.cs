using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PsHandler.PokerTypes;

namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for UCHUDTimer.xaml
    /// </summary>
    public partial class UCHUDTimer : UserControl
    {
        public UCHUDTimer()
        {
            InitializeComponent();

            // Init values

            TextBox_TimerDiff.Text = Config.TimeDiff.ToString(CultureInfo.InvariantCulture);

            // Hook values

            TextBox_TimerDiff.TextChanged += (sender, args) => int.TryParse(TextBox_TimerDiff.Text, out Config.TimeDiff);
        }

        private void Button_CustomizeTimer_Click(object sender, RoutedEventArgs e)
        {
            new WindowCustomizeHud(App.WindowMain).ShowDialog();
        }
    }
}

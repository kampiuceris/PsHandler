using PsHandler.Hud;
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
using PsHandler.Types;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCHud.xaml
    /// </summary>
    public partial class UCHud : UserControl
    {
        public UCHud()
        {
            InitializeComponent();

            // Init values

            //CheckBox_EnableHud.IsChecked = Config.TimerHud;

            TextBox_TimerDiff.Text = Config.TimeDiff.ToString(CultureInfo.InvariantCulture);

            TextBox_TimerDiffLobby.Text = Config.TimeDiffLobby.ToString(CultureInfo.InvariantCulture);

            // Hook values

            CheckBox_EnableHud.Checked += (sender, args) =>
            {
                Button_PokerTypes.IsEnabled = false;
                Button_CustomizeTimer.IsEnabled = false;
                Config.TimerHud = true;
                HudManager.Start();
            };
            CheckBox_EnableHud.Unchecked += (sender, args) =>
            {
                HudManager.Stop();
                Config.TimerHud = false;
                Button_PokerTypes.IsEnabled = true;
                Button_CustomizeTimer.IsEnabled = true;
            };

            TextBox_TimerDiff.TextChanged += (sender, args) => int.TryParse(TextBox_TimerDiff.Text, out Config.TimeDiff);
            TextBox_TimerDiffLobby.TextChanged += (sender, args) => int.TryParse(TextBox_TimerDiffLobby.Text, out Config.TimeDiffLobby);

            // start hud if needed
            CheckBox_EnableHud.IsChecked = Config.TimerHud;
        }

        private void Button_Sync_Click(object sender, RoutedEventArgs e)
        {
            App.LobbyTime.StartSync();
        }

        private void Button_SyncCancel_Click(object sender, RoutedEventArgs e)
        {
            App.LobbyTime.StopSync();
        }

        private void Button_PokerTypes_Click(object sender, RoutedEventArgs e)
        {
            new WindowPokerTypesEdit().ShowDialog();
        }

        private void Button_CustomizeTimer_Click(object sender, RoutedEventArgs e)
        {
            new WindowCustomizeHud().ShowDialog();
        }
    }
}

using PsHandler.Hud;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using PsHandler.PokerTypes;


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

            // start hud if needed
            CheckBox_EnableHud.IsChecked = Config.TimerHud;
        }

        private void Button_PokerTypes_Click(object sender, RoutedEventArgs e)
        {
            new WindowPokerTypesEdit(App.WindowMain).ShowDialog();
        }

        private void Button_CustomizeTimer_Click(object sender, RoutedEventArgs e)
        {
            new WindowCustomizeHud(App.WindowMain).ShowDialog();
        }
    }
}

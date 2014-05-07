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

            // Hook values

            CheckBox_EnableHUD.Checked += (sender, args) =>
            {
                Config.EnableHUD = true;
                TabItem_Timer.IsEnabled = false;
                TabItem_BigBlinds.IsEnabled = false;
                HudManager.Start();
            };
            CheckBox_EnableHUD.Unchecked += (sender, args) =>
            {
                HudManager.Stop();
                TabItem_Timer.IsEnabled = true;
                TabItem_BigBlinds.IsEnabled = true;
                Config.EnableHUD = false;
            };

            // start hud if needed
            CheckBox_EnableHUD.IsChecked = Config.EnableHUD;
        }
    }
}

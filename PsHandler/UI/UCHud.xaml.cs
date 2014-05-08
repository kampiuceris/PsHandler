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
                Config.EnableHud = true;
                HudManager.Start();
            };
            CheckBox_EnableHUD.Unchecked += (sender, args) =>
            {
                HudManager.Stop();
                Config.EnableHud = false;
            };

            // start hud if needed
            CheckBox_EnableHUD.IsChecked = Config.EnableHud;
        }
    }
}

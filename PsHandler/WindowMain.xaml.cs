using System;
using System.Windows;
using System.Windows.Shell;

namespace PsHandler
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        public bool IsClosing;

        public WindowMain()
        {
            InitializeComponent();
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Unknown());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Nova());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Classic());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Black());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Slick());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.HyperSimple());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Stars());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsClosing)
            {
                e.Cancel = true;
                App.Quit();
            }
        }
    }
}

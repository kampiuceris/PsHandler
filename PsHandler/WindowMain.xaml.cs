using System;
using System.Windows;
using System.Windows.Input;
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
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Azure());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Black());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Classic());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.HyperSimple());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Nova());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Slick());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Stars());
            ComboBox_PokerStarsTheme.SelectedIndex = 0;

            foreach (var keyName in Enum.GetValues(typeof(Key)))
            {
                ComboBox_HandReplayHotkey.Items.Add(keyName);
            }
            ComboBox_HandReplayHotkey.SelectedIndex = 0;

            TaskbarIcon_NotifyIcon.TrayMouseDoubleClick += (sender, args) =>
            {
                Show();
                WindowState = WindowState.Normal;
                //MyNotifyIcon.Visibility = Visibility.Hidden;
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsClosing)
            {
                e.Cancel = true;
                App.Quit();
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                if (App.MinimizeToSystemTray)
                {
                    Hide();
                }

                CheckBox_MinimizeToSystemTray.IsEnabled = false;
            }
            else
            {
                CheckBox_MinimizeToSystemTray.IsEnabled = true;
            }
            base.OnStateChanged(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            TaskbarIcon_NotifyIcon.Dispose();
            base.OnClosing(e);
        }
    }
}

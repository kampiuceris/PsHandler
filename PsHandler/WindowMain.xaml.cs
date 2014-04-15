using System;
using System.Windows;
using System.Windows.Controls;
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
            Title = App.NAME + " v" + App.VERSION;

            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Unknown());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Azure());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Black());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Classic());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.HyperSimple());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Nova());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Slick());
            ComboBox_PokerStarsTheme.Items.Add(new PokerStarsThemes.Stars());
            ComboBox_PokerStarsTheme.SelectedIndex = 0;

            TaskbarIcon_NotifyIcon.ContextMenu = GetNotifyIconContextMenu();
            TaskbarIcon_NotifyIcon.ToolTipText = App.NAME + " v" + App.VERSION;
            TaskbarIcon_NotifyIcon.TrayMouseDoubleClick += (sender, args) =>
            {
                Show();
                WindowState = WindowState.Normal;
                //MyNotifyIcon.Visibility = Visibility.Hidden;
            };


            Deactivated += (sender, args) => LoseFocus();
            MouseDown += (sender, args) => LoseFocus();
        }

        private ContextMenu GetNotifyIconContextMenu()
        {
            MenuItem miExit = new MenuItem { Header = "Exit" };
            miExit.Click += (sender, args) => Close();

            ContextMenu cm = new ContextMenu();
            cm.Items.Add(miExit);

            return cm;
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

        private void MenuItem_Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoseFocus()
        {
            // Move to a parent that can take focus
            FrameworkElement parent = (FrameworkElement)Grid_Main.Parent;
            while (parent != null && !((IInputElement)parent).Focusable)
            {
                parent = (FrameworkElement)parent.Parent;
            }
            DependencyObject scope = FocusManager.GetFocusScope(Grid_Main);
            FocusManager.SetFocusedElement(scope, parent);
        }
    }
}

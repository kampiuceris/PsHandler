using System;
using System.Windows;
using System.Windows.Controls;
using PsHandler.PokerTypes;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        public bool IsClosing;

        public bool Importing { set { Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { Grid_Import.Visibility = value ? Visibility.Visible : Visibility.Hidden; })); } }

        public WindowMain()
        {
            InitializeComponent();

            Title = Config.NAME + " v1." + Config.VERSION;

            TaskbarIcon_NotifyIcon.ContextMenu = GetNotifyIconContextMenu();
            TaskbarIcon_NotifyIcon.ToolTipText = Config.NAME + " v1." + Config.VERSION;
            TaskbarIcon_NotifyIcon.TrayMouseDoubleClick += (sender, args) =>
            {
                Show();
                WindowState = WindowState.Normal;
                //MyNotifyIcon.Visibility = Visibility.Hidden;
            };
            Loaded += (sender, args) =>
            {
                if (Config.StartMinimized)
                {
                    WindowState = WindowState.Minimized;
                }
                UCTableTiler.UpdateListView();
                UCPokerTypes.UpdateListView();
            };
        }

        private ContextMenu GetNotifyIconContextMenu()
        {
            MenuItem miExit = new MenuItem { Header = "Exit" };
            miExit.Click += (sender, args) => Close();

            ContextMenu cm = new ContextMenu();
            cm.Items.Add(miExit);

            return cm;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                if (Config.MinimizeToSystemTray)
                {
                    Hide();
                }
                UCSettings.CheckBox_MinimizeToSystemTray.IsEnabled = false;
            }
            else
            {
                UCSettings.CheckBox_MinimizeToSystemTray.IsEnabled = true;
            }
            base.OnStateChanged(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!IsClosing)
            {
                e.Cancel = true;
                App.Quit();
            }
            else
            {
                TaskbarIcon_NotifyIcon.Dispose();
                base.OnClosing(e);
            }

        }
    }
}
